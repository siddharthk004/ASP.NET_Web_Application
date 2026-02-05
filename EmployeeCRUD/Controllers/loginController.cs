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
    public class loginController : BaseController
    {
        private SanmolEntities db = new SanmolEntities();

        // GET: Login
        public ActionResult Index()
        {
            // If already logged in → redirect
            if (Session["LoginId"] != null)
                return RedirectToAction("Index", "Employee");

            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string password)
        {
            // 1️⃣ Basic validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Please enter both username and password";
                return View();
            }

            // 2️⃣ Check login credentials
            var user = db.Logins.FirstOrDefault(x =>
                x.UserName == username &&
                x.Password == password &&
                x.IsActive == true);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid username or password";
                return View();
            }

            // 3️⃣ Check employee status using Eid (FK)
            var employee = db.EEmployees.FirstOrDefault(e => e.LoginId == user.LoginId);

            if (employee == null || employee.IsActive == false)
            {
                ViewBag.ErrorMessage = "Employee is inactive. Contact administrator.";
                return View();
            }

            // 4️⃣ Store SESSION (ONLY REQUIRED DATA)
            Session["LoginId"] = user.LoginId;
            Session["EmpId"] = employee.Eid;
            Session["UserName"] = user.UserName;
            Session["RoleId"] = user.RoleId;

            return RedirectToAction("Index", "Employee");
        }

        // LOGOUT
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }

}

