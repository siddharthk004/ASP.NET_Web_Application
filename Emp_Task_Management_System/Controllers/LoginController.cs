using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Emp_Task_Management_System.Models;

namespace Emp_Task_Management_System.Controllers
{
    public class LoginController : Controller
    {
        private readonly ETMSEntities db = new ETMSEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Registration model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Registrations.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Store user information in session
                    Session["UserId"] = user.Id;
                    Session["Username"] = user.Username;
                    Session["Email"] = user.Email;

                    // Redirect to home page or dashboard
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid username or password. Please try again.";
                }
            }

            return View("Index", model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
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
