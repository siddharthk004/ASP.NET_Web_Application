using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeeCRUD.Models;

namespace EmployeeCRUD.Controllers
{
    public class loginController : Controller
    {
        private SanmolEntities db = new SanmolEntities();

        // GET: Login Page
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Please enter both username and password";
                return View();
            }

            var user = db.Logins.FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user != null)
            {
                // Store user info in session
                Session["UserId"] = user.Id;
                Session["EmpId"] = user.Eid;
                Session["UserName"] = user.UserName;
                Session["Password"] = user.Password;
                Session["UserRole"] = user.Role;

                // Redirect to Employee Index page
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password";
                return View();
            }
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "login");
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

