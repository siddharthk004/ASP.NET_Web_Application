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
    public class RegisterController : Controller
    {
        private readonly ETMSEntities db = new ETMSEntities();

        
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,Email,Designation,IsActive")] Registration credential)
        {
            if (ModelState.IsValid)
            {
                credential.CreatedAt = DateTime.Now;
                db.Registrations.Add(credential);
                db.SaveChanges();
                return RedirectToAction("Index","Login");
            }

            return View(credential);
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
