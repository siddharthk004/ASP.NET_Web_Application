using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Role_Base_Authentication.Models;

namespace Role_Base_Authentication.Controllers
{
    public class registrationController : Controller
    {
        private SanmolEnt db = new SanmolEnt();

        // GET: registration
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Name, string Mobile, string Email, string Role, string Password, string ConfirmPassword)
        {
            // Validate passwords match
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }


            // Check if email already exists
            if (db.registrations.Any(u => u.email == Email))
            {
                ViewBag.Error = "Email already exists";
                return View();
            }

            // Create new registration
            registration newUser = new registration
            {
                name = Name,
                contact = Mobile,
                email = Email,
                role = Role,
                password = Password,
                Cpassword = ConfirmPassword
            };

            db.registrations.Add(newUser);
            db.SaveChanges();

            ViewBag.Success = "Registration successful! Please login.";
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
