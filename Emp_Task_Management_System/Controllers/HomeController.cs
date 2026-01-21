using Emp_Task_Management_System.Models;
using System.Linq;
using System.Web.Mvc;

namespace Emp_Task_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ETMSEntities db = new ETMSEntities();

        public ActionResult Index()
        {
            ViewBag.TotalTask = db.Tasks.Count();
            ViewBag.TotalEmployee = db.Registrations.Count();
            ViewBag.ActiveEmp = db.Registrations.Where(u => u.IsActive).Count();
            ViewBag.InProgressTask= db.Tasks.Where(u => u.Status == true).Count();

            ViewBag.Registrations = db.Registrations.ToList();
            ViewBag.Tasks = db.Tasks.ToList();
            return View(db.Registrations.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}