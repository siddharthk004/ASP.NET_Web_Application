using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using EmployeeCRUD.Models;

namespace EmployeeCRUD.Controllers
{
    public class EmployeeController : Controller
    {
        private SanmolEntities db = new SanmolEntities();

        public enum EmployeeType
        {
            Permanant = 1,
            Full_Time = 2,
            Part_Time = 3,
            Internship = 4,
            Freelance = 5,
            Contract = 6,
            Volunterring = 7
        }

        #region Index
        public ActionResult Index(int top = 10,string searchtxt = null,string sortBy = "Ename",string sortDir = "asc",int page = 1,int pageSize = 10)
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
        #endregion


        #region Home Screen
        public ActionResult Details(int? id = null)
        {
            Employee model;

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            model = db.Employees.FirstOrDefault(x => x.Eid == id.Value);
            if (model == null)
                return HttpNotFound();

            return View("Details", model);
        }

        public ActionResult PopUp(int? id, char mode)
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
        #endregion


        #region Profile View
        public ActionResult ProfileView(int? id = null)
        {
            Employee model;

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            model = db.Employees.FirstOrDefault(x => x.Eid == id.Value);
            if (model == null)
                return HttpNotFound();

            return View("Profile",model);
        }

        public ActionResult ProfileTab(String tab = null, int? id = null)
        {
            var data = db.Employees.FirstOrDefault(x => x.Eid == id); if (data == null) return HttpNotFound();
            if (id == null)
                return HttpNotFound();

            switch (tab)
            {
                case "BasicInfo":
                    var employee = db.Employees.FirstOrDefault(x => x.Eid == id);
                    if (employee == null)
                        return HttpNotFound();

                    var basicInfo = db.BasicInfoes
                                      .FirstOrDefault(x => x.Eid == id);

                    // If BasicInfo doesn't exist, create a new empty one for the form
                    if (basicInfo == null)
                    {
                        basicInfo = new BasicInfo
                        {
                            Eid = id.Value
                        };
                    }

                    var vm = new EmployeeVM
                    {
                        Employee = employee,
                        BasicInfo = basicInfo
                    };
                    return PartialView("_BasicInfo", vm);


                case "EmploymentHistory":
                    employee = db.Employees.FirstOrDefault(x => x.Eid == id);
                    if (employee == null)
                        return HttpNotFound();

                    var EmpHistory = db.EmployeeEmploymentHistories
                        .Where(x => x.EmployeeId == id)
                        .ToList();
                    System.Diagnostics.Debug.WriteLine("Employee ID: " + id);

                    ViewBag.id = id;

                    var EHVM = new EmployeeVM
                    {
                        Employee = employee,
                        EmploymentHistory = EmpHistory
                    };
                    return PartialView("_EmploymentHistory", EHVM);


                case "Assessments":
                    return PartialView("_Assessments", data);


                case "EducationHistory":
                    employee = db.Employees.FirstOrDefault(x => x.Eid == id);
                    if (employee == null)
                        return HttpNotFound();

                    var EmpEduHistory = db.EmployeeEducationHistories
                        .Where(x => x.EmployeeId == id)
                        .ToList();

                    var EEHVM = new EmployeeVM
                    {
                        Employee = employee,
                        EducationHistory = EmpEduHistory
                    };
                    return PartialView("_EducationHistory", EEHVM);


                case "Licenses":
                    return PartialView("_Licenses", data);
                case "Trainings":
                    return PartialView("_Trainings", data);
                case "Leaves":
                    return PartialView("_Leaves", data);
                case "LeaveBalance":
                    return PartialView("_LeaveBalance", data);
                case "Remuneration":
                    return PartialView("_Remuneration", data);
                case "References":
                    return PartialView("_References", data);
                case "Pensions":
                    return PartialView("_Pensions", data);
                case "PaySlips":
                    return PartialView("_PaySlips", data);
                case "Loans":
                    return PartialView("_Loans", data);
                case "Insurances":
                    return PartialView("_Insurances", data);
                case "Family":
                    return PartialView("_Family", data);
                case "EmergencyContact":
                    return PartialView("_EmergencyContact", data);
                case "DisciplinaryLogs":
                    return PartialView("_DisciplinaryLogs", data);
                case "ConcessionPasses":
                    return PartialView("_ConcessionPasses", data);
                case "Banks":
                    return PartialView("_Banks", data);
                case "TimeZone":
                    return PartialView("_TimeZone", data);
                default:
                    return PartialView("_BasicInfo", data);
            }
        }



        #region EmployeeHistory
        [HttpGet]
        public ActionResult AddEmploymentHistory(int employeeId)
        {

            ViewBag.EmploymentTypes = Enum.GetValues(typeof(EmployeeType))
                                  .Cast<EmployeeType>()
                                  .Select(e => new SelectListItem
                                  {
                                      Text = e.ToString(),
                                      Value = ((int)e).ToString()
                                  }).ToList();

            var model = new EmployeeEmploymentHistory
            {
                EmployeeId = employeeId,
                DateOfJoining = DateTime.Today,
                IsActive = true
            };

            return PartialView("AddEmploymentHistoryForm", model);
        }

        [HttpPost]
        public ActionResult AddEmploymentHistory(EmployeeEmploymentHistory model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });
            
            var emp = db.Employees.FirstOrDefault(x => x.Eid == model.EmployeeId);
            model.CreatedBy = emp.Ename;
            model.CreatedDate = DateTime.Now;

            db.EmployeeEmploymentHistories.Add(model);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult DeleteEmployeeHistory(int id)
        {
            var history = db.EmployeeEmploymentHistories
                            .FirstOrDefault(x => x.EmploymentHistoryId == id);

            if (history == null)
                return Json(new { success = false, message = "Record not found" });

            db.EmployeeEmploymentHistories.Remove(history);
            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult EditEmploymentHistory(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var history = db.EmployeeEmploymentHistories
                            .FirstOrDefault(x => x.EmploymentHistoryId == id.Value);

            if (history == null)
                return HttpNotFound();


            ViewBag.EmploymentTypes = Enum.GetValues(typeof(EmployeeType))
                                  .Cast<EmployeeType>()
                                  .Select(e => new SelectListItem
                                  {
                                      Text = e.ToString(),
                                      Value = ((int)e).ToString()
                                  }).ToList();
            return PartialView("EditEmploymentHistoryForm", history);
        }

        [HttpPost]
        public ActionResult EditEmploymentHistory(EmployeeEmploymentHistory model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            var history = db.EmployeeEmploymentHistories
                            .FirstOrDefault(x => x.EmploymentHistoryId == model.EmploymentHistoryId);

            if (history == null)
                return Json(new { success = false, message = "Record not found" });

            history.EmployerName = model.EmployerName;
            history.Designation = model.Designation;
            history.DateOfJoining = model.DateOfJoining;
            history.DateOfExit = model.DateOfExit;
            history.EmploymentType = model.EmploymentType;
            history.ExitReason = model.ExitReason;
            history.RolePerformed = model.RolePerformed;
            history.IsActive = model.IsActive;

            var emp = db.Employees.FirstOrDefault(x => x.Eid == model.EmployeeId);
            if (emp != null)
            {
                history.UpdatedBy = emp.Ename;
            }
            history.UpdatedDate = DateTime.Now;

            db.SaveChanges();

            return Json(new { success = true });
        }
        #endregion EmployeeHistory
        
        
        #endregion Profile View


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
