using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LifeOS.Controllers
{
    public class TaskController : Controller
    {
        LifeOSContext db = new LifeOSContext();

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];
            var tasks = db.Tasks
                         .Where(t => t.UserId == userId)
                         .OrderBy(t => t.DueDateTime)
                         .ToList();

            return View(tasks);
        }

        [HttpPost]
        public ActionResult Add(string title, string description, DateTime dueDateTime, string priority)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];

            db.Tasks.Add(new TaskItem
            {
                UserId = userId,
                Title = title,
                Description = description,
                DueDateTime = dueDateTime,
                Priority = priority ?? "Medium",
                Status = "Pending",
                CreatedAt = DateTime.Now
            });

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Complete(int id)
        {
            var task = db.Tasks.Find(id);
            if (task != null && task.Status == "Pending")
            {
                task.Status = "Done";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var task = db.Tasks.Find(id);
            if (task != null)
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}