using System;
using System.Linq;
using System.Web.Mvc;

namespace LifeOS.Controllers
{
    public class MoodController : Controller
    {
        LifeOSContext db = new LifeOSContext();

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];
            var today = DateTime.Today;
            
            var moodLogs = db.MoodLogs
                            .Where(m => m.UserId == userId && m.MoodDate >= today.AddDays(-30))
                            .OrderByDescending(m => m.MoodDate)
                            .ToList();

            ViewBag.AverageMood = moodLogs.Any() ? moodLogs.Average(m => m.MoodLevel) : 0;

            return View(moodLogs);
        }

        [HttpPost]
        public ActionResult Add(int moodLevel)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];
            var today = DateTime.Today;

            var existing = db.MoodLogs.FirstOrDefault(m => m.UserId == userId && m.MoodDate == today);

            if (existing != null)
            {
                existing.MoodLevel = moodLevel;
            }
            else
            {
                db.MoodLogs.Add(new MoodLog
                {
                    UserId = userId,
                    MoodLevel = moodLevel,
                    MoodDate = today
                });
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var log = db.MoodLogs.Find(id);
            if (log != null)
            {
                db.MoodLogs.Remove(log);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
