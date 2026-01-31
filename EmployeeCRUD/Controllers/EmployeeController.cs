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
    public class EmployeeController : Controller
    {
        private SanmolEntities db = new SanmolEntities();
        public ActionResult Index(
    int top = 10,
    string searchtxt = null,
    string sortBy = "Ename",
    string sortDir = "asc",
    int page = 1,
    int pageSize = 10)
        {
            IQueryable<Employee> query = db.Employees;

            if (!string.IsNullOrWhiteSpace(searchtxt))
            {
                query = query.Where(x => x.Ename.Contains(searchtxt));
            }

            switch (sortBy)
            {
                case "Eid":
                    query = sortDir == "asc"
                        ? query.OrderBy(x => x.Eid)
                        : query.OrderByDescending(x => x.Eid);
                    break;

                case "Ename":
                    query = sortDir == "asc"
                        ? query.OrderBy(x => x.Ename)
                        : query.OrderByDescending(x => x.Ename);
                    break;

                case "Etype":
                    query = sortDir == "asc"
                        ? query.OrderBy(x => x.Etype)
                        : query.OrderByDescending(x => x.Etype);
                    break;

                case "Eaddr":
                    query = sortDir == "asc"
                        ? query.OrderBy(x => x.Eaddr)
                        : query.OrderByDescending(x => x.Eaddr);
                    break;

                case "Emob":
                    query = sortDir == "asc"
                        ? query.OrderBy(x => x.Emob)
                        : query.OrderByDescending(x => x.Emob);
                    break;

                case "Edesign":
                    query = sortDir == "asc"
                        ? query.OrderBy(x => x.Edesign)
                        : query.OrderByDescending(x => x.Edesign);
                    break;

                default:
                    query = query.OrderBy(x => x.Ename);
                    break;
            }

            if (top > 0)
            {
                query = query.Take(top);
            }

            int totalRecords = query.Count();
            var data = query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            
            ViewBag.SortBy = sortBy;
            ViewBag.SortDir = sortDir;
            ViewBag.Top = top;
            ViewBag.SearchTxt = searchtxt;

            return View(query.ToList());
        }



        [HttpGet]
        public ActionResult Delete(int id)
        {
            var employee = db.Employees.Find(id);
            if (employee != null)
            {
                db.Employees.Remove(employee);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Created()
        {
            return PartialView("Created", new Employee());
        }

        public ActionResult Details(int? id=null)
        {
            Employee model;

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            model = db.Employees.FirstOrDefault(x => x.Eid == id.Value);
            if (model == null)
                return HttpNotFound();

            return View("Details", model);
        }

        public ActionResult PopUp(int? id , char mode)
        {
            Employee model;
            if (mode == 'e') 
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                
                model = db.Employees.FirstOrDefault(x => x.Eid == id.Value);
                if (model == null)
                    return HttpNotFound();
                
                return PartialView("Edit", model);
            }
            else if (mode == 'v') 
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                
                model = db.Employees.FirstOrDefault(x => x.Eid == id.Value);
                if (model == null)
                    return HttpNotFound();
                
                return PartialView("Details", model);
            }
            else 
            {
                return PartialView("Create", new Employee());
            }
        }
        public ActionResult Edit(int? id = null)
        {
            var emp = db.Employees.Find(id);
            if (emp == null) return HttpNotFound();
            return View("Edit", emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            try
            {
                var emp = db.Employees.Find(model.Eid);
                if (emp == null)
                {
                    return Json(new { success = false, message = "Employee not found" });
                }

                emp.Ename = model.Ename;
                emp.Etype = model.Etype;
                emp.Eaddr = model.Eaddr;
                emp.Emob = model.Emob;
                emp.Edesign = model.Edesign;
                emp.IsActive = model.IsActive;

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Save(Employee model)
        {
            if (!ModelState.IsValid)
                return PartialView("Create", model);

            if (model.Eid == 0) 
            {
                try
                {
                    db.Employees.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    throw;
                }
            }
            else 
            {
                var emp = db.Employees.First(x => x.Eid == model.Eid);
                emp.Ename = model.Ename;
                emp.Etype = model.Etype;
                emp.Eaddr = model.Eaddr;
                emp.Emob = model.Emob;
                emp.Edesign = model.Edesign;
                emp.IsActive = model.IsActive;
                
                db.SaveChanges();
                return Json(new { success = true });
            }
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
