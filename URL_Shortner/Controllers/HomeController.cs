using System;
using System.Linq;
using System.Web.Mvc;
using URL_Shortner.Models;

namespace URL_Shortner.Controllers
{
    public class HomeController : Controller
    {
        private readonly URL_ShortnerEntities db = new URL_ShortnerEntities();

        public ActionResult Index()
        {
            return View();
        }

        private static readonly Random _random = new Random();

        private string GenerateShortCode(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[_random.Next(s.Length)])
                          .ToArray()
            );
        }

        [HttpPost]
        public ActionResult CreateShortUrl(string longUrl, string customCode = null)
        {
            if (string.IsNullOrWhiteSpace(longUrl))
                return Json(new { success = false, message = "URL is required" });

            // Validate URL format
            if (!Uri.IsWellFormedUriString(longUrl, UriKind.Absolute))
                return Json(new { success = false, message = "Please enter a valid URL format" });

            string shortCode;

            // Check if custom code is provided
            if (!string.IsNullOrWhiteSpace(customCode))
            {
                // Validate custom code (alphanumeric only)
                if (!System.Text.RegularExpressions.Regex.IsMatch(customCode, "^[a-zA-Z0-9]+$"))
                    return Json(new { success = false, message = "Custom code can only contain letters and numbers" });

                // Check minimum length
                if (customCode.Length < 3)
                    return Json(new { success = false, message = "Custom code must be at least 3 characters long" });

                // Check if custom code is already taken
                if (db.ShortUrls.Any(x => x.ShortCode == customCode))
                    return Json(new { success = false, message = "Custom code '" + customCode + "' is already taken. Please try another." });

                shortCode = customCode;
            }
            else
            {
                // Generate random code
                do
                {
                    shortCode = GenerateShortCode();
                }
                while (db.ShortUrls.Any(x => x.ShortCode == shortCode));
            }

            var shortUrl = new ShortUrl
            {
                OriginalUrl = longUrl,
                ShortCode = shortCode,
                CreatedOn = DateTime.Now,
                ClickCount = 0,
                IsActive = true
            };

            db.ShortUrls.Add(shortUrl);
            db.SaveChanges();

            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string resultUrl = baseUrl + "/u/" + shortCode;

            return Json(new
            {
                success = true,
                shortUrl = resultUrl
            });
        }

        [HttpGet]
        [Route("u/{code}")]
        public ActionResult Go(string code)
        {
            var record = db.ShortUrls.FirstOrDefault(x =>
                x.ShortCode == code && x.IsActive);

            if (record == null)
                return HttpNotFound("Invalid or expired URL");

            if (record.ExpiryDate != null && record.ExpiryDate < DateTime.Now)
                return Content("This link has expired");

            record.ClickCount++;
            db.SaveChanges();

            return Redirect(record.OriginalUrl);
        }

    }
}
