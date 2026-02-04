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
            //IQueryable<Employee> query = db.Employees.Where(x => x.IsActive);

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
                employee.IsActive = false;
                //db.Employees.Remove(employee);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Created()
        {

            ViewBag.EmploymentTypes = db.EmployeeTypeMasters
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                })
                .ToList();

            ViewBag.Roles = db.Roles
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                })
                .ToList();
            return PartialView("Created", new Employee());
        }

        [HttpPost]
        public JsonResult CheckUsernameAvailability(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Json(new { available = false, message = "Username is required" });

            // Check in logins table
            var existsInLogins = db.Logins.Any(l => l.UserName.ToLower() == username.ToLower());

            bool isAvailable = !existsInLogins;

            return Json(new
            {
                available = isAvailable,
                message = isAvailable ? "Username is available" : "Username already exists"
            });
        }
      
        [HttpGet]
        public JsonResult IsUsernameAvailable(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Json(true, JsonRequestBehavior.AllowGet);

            bool exists = db.Logins.Any(x => x.UserName == username);

            // true = valid, false = already exists
            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? id = null)
        {
            var emp = db.Employees.Find(id);
            if (emp == null) return HttpNotFound();

            // Populate dropdown data
            ViewBag.EmploymentTypes = db.EmployeeTypeMasters
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name,
                    Selected = x.Name == emp.Etype
                })
                .ToList();

            // Get login info by Employee ID (not by username)
            var userLogin = db.Logins.FirstOrDefault(l => l.Eid == id);
            var mail = db.BasicInfoes.FirstOrDefault(l => l.Eid == id);

            ViewBag.UserRole = userLogin != null ? userLogin.Role : "Not Assigned";
            ViewBag.UserNameVB = userLogin != null ? userLogin.UserName : emp.Ename;  // Fallback to emp.Ename
            ViewBag.Mail = mail != null ? mail.PersonalEmail : "";

            return View("Edit", emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model, string username, string password, string email)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Update Employee
                    var emp = db.Employees.Find(model.Eid);
                    if (emp == null)
                    {
                        return Json(new { success = false, message = "Employee not found" });
                    }

                    emp.Ename = model.Ename;
                    emp.Etype = model.Etype;
                    emp.Eaddr = model.Eaddr ?? "";
                    emp.Emob = model.Emob ?? "";
                    emp.Edesign = model.Edesign ?? "";
                    emp.IsActive = model.IsActive;

                    // 2. Update Password if provided
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        var login = db.Logins.FirstOrDefault(l => l.Eid == model.Eid);
                        if (login != null)
                        {
                            login.Password = password; // TODO: Hash this!
                        }
                    }

                    // 3. Update Email if changed
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var basicInfo = db.BasicInfoes.FirstOrDefault(b => b.Eid == model.Eid);
                        if (basicInfo != null)
                        {
                            basicInfo.PersonalEmail = email;
                        }
                        else
                        {
                            // Create BasicInfo if doesn't exist
                            db.BasicInfoes.Add(new BasicInfo
                            {
                                Eid = model.Eid,
                                PersonalEmail = email,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine($"Edit Error: {ex.Message}");
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
        
        
        [HttpPost]
        public ActionResult Save(Employee model, string username, string password, String role, string email)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"Save called - Username: {username}, Password: {password}, Role: {role}, Email: {email}");
            System.Diagnostics.Debug.WriteLine($"Model - Ename: {model.Ename}, Etype: {model.Etype}, IsActive: {model.IsActive}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Invalid data: " + string.Join(", ", errors) });
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Validate required fields
                    if (string.IsNullOrWhiteSpace(username))
                        return Json(new { success = false, message = "Username is required" });

                    if (string.IsNullOrWhiteSpace(password))
                        return Json(new { success = false, message = "Password is required" });

                    if (string.IsNullOrWhiteSpace(role))
                        return Json(new { success = false, message = "Role is required" });

                    // 2. Check username uniqueness
                    bool usernameExists = db.Logins.Any(x => x.UserName.ToLower() == username.ToLower());
                    if (usernameExists)
                    {
                        return Json(new { success = false, message = "Username already exists" });
                    }

                    // 3. Save Employee
                    var employee = new Employee
                    {
                        Ename = model.Ename, 
                        Etype = model.Etype,
                        Eaddr = model.Eaddr ?? "", 
                        Emob = model.Emob ?? "",
                        Edesign = model.Edesign ?? "",
                        IsActive = model.IsActive
                    };

                    db.Employees.Add(employee);
                    db.SaveChanges(); 

                    int employeeId = employee.Eid;

                    // 4. Save Login
                    var login = new Login
                    {
                        UserName = username,
                        Password = password,  
                        Role = role,
                        Eid = employeeId
                    };

                    db.Logins.Add(login);
                    db.SaveChanges();

                    // 5. Save Basic Info (Email) - Only if email provided
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var basicInfo = new BasicInfo
                        {
                            Eid = employeeId,
                            PersonalEmail = email,
                            CreatedDate = DateTime.Now
                        };

                        db.BasicInfoes.Add(basicInfo);
                        db.SaveChanges();
                    }

                    // 6. Commit all
                    transaction.Commit();

                    return Json(new { success = true });
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    transaction.Rollback();

                    // Get detailed validation errors
                    var errorMessages = new List<string>();
                    foreach (var validationError in ex.EntityValidationErrors)
                    {
                        var entityName = validationError.Entry.Entity.GetType().Name;
                        foreach (var error in validationError.ValidationErrors)
                        {
                            errorMessages.Add($"{entityName}.{error.PropertyName}: {error.ErrorMessage}");
                            System.Diagnostics.Debug.WriteLine($"Validation Error - {entityName}.{error.PropertyName}: {error.ErrorMessage}");
                        }
                    }

                    return Json(new
                    {
                        success = false,
                        message = "Validation failed: " + string.Join("; ", errorMessages)
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                    return Json(new
                    {
                        success = false,
                        message = ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")
                    });
                }
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
            ViewBag.EmploymentTypes = db.EmployeeTypeMasters
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                })
                .ToList();

            ViewBag.EmployeeTypes = Enum.GetValues(typeof(EmployeeType))
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
            bool result = DeleteEntity<EmployeeEmploymentHistory>(x => x.EmploymentHistoryId == id);

            if (!result)
                return Json(new { success = false, message = "Record not found" });

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

            #region enum
            //ViewBag.EmploymentTypes = Enum.GetValues(typeof(EmployeeType))
            //                      .Cast<EmployeeType>()
            //                      .Select(e => new SelectListItem
            //                      {
            //                          Text = e.ToString(),
            //                          Value = ((int)e).ToString()
            //                      }).ToList();
            #endregion

            ViewBag.EmploymentTypes = db.EmployeeTypeMasters
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                })
                .ToList();
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

        [HttpGet]
        public ActionResult SearchEmploymentHistory(int employeeId, string search)
        {
            var employee = db.Employees.FirstOrDefault(x => x.Eid == employeeId);
            if (employee == null)
                return HttpNotFound();

            var history = db.EmployeeEmploymentHistories
                .Where(x => x.EmployeeId == employeeId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                history = history.Where(x =>
                    x.EmployerName.Contains(search) ||
                    x.Designation.Contains(search) ||
                    x.RolePerformed.Contains(search)
                );
            }

            var vm = new EmployeeVM
            {
                Employee = employee,
                EmploymentHistory = history
                    .OrderByDescending(x => x.DateOfJoining) // ✅ keep sorting
                    .ToList()
            };

            return PartialView("_EmploymentHistoryRows", vm);
        }

        #endregion EmployeeHistory

        #region View Permissions

        [HttpGet]
        public ActionResult ViewPermissions(int employeeId, string role)
        {
            var employee = db.Employees.Find(employeeId);
            if (employee == null)
                return HttpNotFound();

            ViewBag.EmployeeName = employee.Ename;
            ViewBag.EmployeeId = employeeId;
            ViewBag.UserRoles = role;
            
            return View();
        }

        #endregion View Permissions

        #endregion Profile View

        public bool DeleteEntity<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            var entity = db.Set<TEntity>().FirstOrDefault(predicate);
            if (entity == null)
                return false;

            db.Set<TEntity>().Remove(entity);
            db.SaveChanges();
            return true;
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
