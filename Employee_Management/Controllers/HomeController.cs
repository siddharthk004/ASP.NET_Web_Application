using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee_Management.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxDemo()
        {
            return View();
        }

        public JsonResult GetEmployee()
        {
            var emp = new
            {
                name = "Siddharth Kardile",
                Designation = "API Developer",
                Location = "Pune",
                Company = "Sanmol Software"
            };
            return Json(emp,JsonRequestBehavior.AllowGet);
        }

    }
}