using System.Linq;
using System.Web.Mvc;
using Role_Base_Authentication.Models;

namespace Role_Base_Authentication.Controllers
{
    public class loginController : Controller
    {
        private SanmolEnt db = new SanmolEnt();

        // GET: login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Name, string Password)
        {
            // Find user with matching name & password
            var user = db.registrations
                         .FirstOrDefault(u => u.name == Name && u.password == Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // Store session
            Session["UserName"] = user.name;
            Session["Role"] = user.role;

            // Role-based redirect
            if (user.role == "Admin")
                return RedirectToAction("Index", "Admin");
            else
                return RedirectToAction("Index", "User");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
