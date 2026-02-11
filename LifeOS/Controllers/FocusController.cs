using System;
using System.Linq;
using System.Web.Mvc;

namespace LifeOS.Controllers
{
    public class FocusController : Controller
    {
        LifeOSContext db = new LifeOSContext();

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];
            var today = DateTime.Today;
            
            var focusLogs = db.FocusLogs
                             .Where(f => f.UserId == userId && f.LogDate >= today.AddDays(-7))
                             .OrderByDescending(f => f.LogDate)
                             .ToList();

            ViewBag.TodayTotal = focusLogs.Where(f => f.LogDate >= today).Sum(f => f.MinutesSpent);

            return View(focusLogs);
        }

        [HttpPost]
        public ActionResult Add(string category, int minutesSpent)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];

            db.FocusLogs.Add(new FocusLog
            {
                UserId = userId,
                Category = category,
                MinutesSpent = minutesSpent,
                LogDate = DateTime.Now
            });

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var log = db.FocusLogs.Find(id);
            if (log != null)
            {
                db.FocusLogs.Remove(log);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
