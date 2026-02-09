using EmployeeCRUD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EmployeeCRUD.Controllers
{
    public class EmployeeController : BaseController
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
     
        public ActionResult Index(  int? top = null,  string searchtxt = null,  string sortBy = "Ename", string sortDir = "asc")
        {
            int roleId = Convert.ToInt32(Session["RoleId"]);
            int eid = Convert.ToInt32(Session["EmpId"]);

            IQueryable<EEmployee> query =
                db.EEmployees.Include(e => e.EmployeeTypeMaster);

            /* =========================
               1️⃣ READ PERMISSION (STRICT)
               ========================= */
            var readableTypeIds = db.UserRoleTypePermissions
                .Where(p => p.Eid == eid && p.CanRead && p.IsActive)
                .Select(p => p.TypeId)
                .Distinct()
                .ToList();

            if (!readableTypeIds.Any())
            {
                // No readable types → no data
                query = query.Where(x => false);
            }

            /* =========================
               2️⃣ ADMIN BYPASS
               ========================= */
            if (roleId != 1 && readableTypeIds.Any())
            {
                // filter only readable types
                query = query.Where(x => readableTypeIds.Contains(x.TypeId));

                var menuPermission = db.UserRoleMenuViews
                    .FirstOrDefault(m =>
                        m.IsActive &&
                        m.RolesTypesView.RoleId == roleId);

                if (menuPermission != null)
                {
                    if (menuPermission.ViewType == 0)
                        query = query.Where(x => x.IsActive);
                    else if (menuPermission.ViewType == 1)
                        query = query.Where(x => !x.IsActive);
                }
            }

            /* =========================
               3️⃣ SEARCH
               ========================= */
            if (!string.IsNullOrWhiteSpace(searchtxt))
            {
                query = query.Where(x => x.FullName.Contains(searchtxt));
            }

            /* =========================
               4️⃣ SORTING
               ========================= */
            switch (sortBy)
            {
                case "Eid":
                    query = sortDir == "asc"
                        ? query.OrderBy(x => x.Eid)
                        : query.OrderByDescending(x => x.Eid);
                    break;

                case "Ename":
                default:
                    query = sortDir == "asc"
                        ? query.OrderBy(x => x.FullName)
                        : query.OrderByDescending(x => x.FullName);
                    break;
            }

            var data = query.ToList();

            /* =========================
               5️⃣ VIEW DATA
               ========================= */
            ViewBag.CanCreate = db.UserRoleTypePermissions
                .Any(p => p.Eid == eid && p.CanCreate && p.IsActive);

            ViewBag.PermissionList = db.UserRoleTypePermissions
                .Where(p => p.Eid == eid && p.IsActive)
                .Select(p => new TypePermissionDto
                {
                    TypeId = p.TypeId,
                    CanRead = p.CanRead,
                    CanUpdate = p.CanUpdate,
                    CanDelete = p.CanDelete
                })
                .ToList();

            ViewBag.SortBy = sortBy;
            ViewBag.SortDir = sortDir;

            return View(data);
        }
    
        #endregion

        #region Home Screen
        public ActionResult Details(int? id = null)
        {
            EEmployee model;

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            model = db.EEmployees.FirstOrDefault(x => x.Eid == id.Value);
            if (model == null)
                return HttpNotFound();

            return View("Details", model);
        }

        public ActionResult PopUp(int? id, char mode)
        {
            EEmployee model;
            if (mode == 'e')
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                model = db.EEmployees.FirstOrDefault(x => x.Eid == id.Value);
                if (model == null)
                    return HttpNotFound();

                return PartialView("Edit", model);
            }
            else if (mode == 'v')
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                model = db.EEmployees.FirstOrDefault(x => x.Eid == id.Value);
                if (model == null)
                    return HttpNotFound();

                return PartialView("Details", model);
            }
            else
            {
                return PartialView("Create", new EEmployee());
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var employee = db.EEmployees.Find(id);
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
            int eid = Convert.ToInt32(Session["EmpId"]);
            int currentRoleId = Convert.ToInt32(Session["RoleId"]);

            var allowedTypeIds = db.UserRoleTypePermissions
                .Where(p => p.Eid == eid && p.CanCreate && p.IsActive)
                .Select(p => p.TypeId)
                .Distinct()
                .ToList();

            ViewBag.EmploymentTypes = db.EmployeeTypeMasters
                .Where(x => allowedTypeIds.Contains(x.Id))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();

            ViewBag.Roles = db.Roles
                .Where(x =>
                    !string.IsNullOrEmpty(x.Name) &&
                    (currentRoleId == 1 || x.RoleId != 1)
                )
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.RoleId.ToString()
                })
                .ToList();

            return PartialView("Created", new EEmployee());
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

            int currentUserRoleId = Convert.ToInt32(Session["RoleId"]);

            // flag for view (optional but clean)
            ViewBag.IsAdmin = (currentUserRoleId == 1);

            // build roles list conditionally
            ViewBag.Roles = db.Roles
                .Where(r =>
                    (
                        currentUserRoleId == 1      // ADMIN → all roles
                        || r.RoleId != 1            // NON-ADMIN → exclude Admin
                    )
                )
                .ToList() // Materialize the query to avoid dynamic in expression tree
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.RoleId.ToString(),
                    Selected = false // Set to false or handle selection in the view
                })
                .ToList();



            var emp = db.EEmployees.Find(id);
            if (emp == null) return HttpNotFound();

            //int eid = Convert.ToInt32(Session["EmpId"]);

            var allowedTypeIds = db.UserRoleTypePermissions
                .Where(p => p.Eid == id && p.CanUpdate && p.IsActive)
                .Select(p => p.TypeId)
                .Distinct()
                .ToList();

            ViewBag.EmploymentTypes = db.EmployeeTypeMasters
                .Where(x => allowedTypeIds.Contains(x.Id))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();

            var mail = db.BasicInfoes.FirstOrDefault(l => l.Eid == id);

            var emps = db.EEmployees
            .Include(e => e.Login)
            .FirstOrDefault(e => e.Eid == id);

            if (emps == null || emps.Login == null)
            {
                ViewBag.UserRole = "Not Assigned";
                ViewBag.CurrentRoleId = 0;
            }
            else
            {
                int roleId = emps.Login.RoleId;

                var role = db.Roles
                             .FirstOrDefault(r => r.RoleId == roleId);

                ViewBag.UserRole = role != null ? role.Name : "Not Assigned";
                ViewBag.CurrentRoleId = roleId;
            }
            var userLoginId = db.EEmployees.FirstOrDefault(l => l.Eid == id);
            var userLogin = db.Logins.FirstOrDefault(l => l.LoginId == userLoginId.LoginId);
            ViewBag.UserNameVB = userLogin.UserName;
            ViewBag.Mail = mail != null ? mail.PersonalEmail : "";

            return View("Edit", emp);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EEmployee model, string password, string email, int? roleId)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Load Employee with Login
                    var emp = db.EEmployees
                                .Include(e => e.Login)
                                .FirstOrDefault(e => e.Eid == model.Eid);

                    if (emp == null)
                        return Json(new { success = false, message = "Employee not found" });

                    // 2️⃣ Update Employee fields
                    emp.FullName = model.FullName;
                    emp.TypeId = model.TypeId;
                    emp.Address = model.Address ?? "";
                    emp.Mobile = model.Mobile ?? "";
                    emp.Designation = model.Designation ?? "";
                    emp.UpdatedAt = DateTime.Now;

                    // 3️⃣ Update password ONLY if provided
                    if (!string.IsNullOrWhiteSpace(password) && emp.Login != null)
                    {
                        emp.Login.Password = password; // ⚠ hash later
                    }

                    // 4️⃣ Update role if provided and user is admin
                    int currentUserRoleId = Convert.ToInt32(Session["RoleId"]);
                    if (roleId.HasValue && currentUserRoleId == 1 && emp.Login != null)
                    {
                        emp.Login.RoleId = roleId.Value;
                    }

                    // 5️⃣ Update / Insert BasicInfo (Email)
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var basicInfo = db.BasicInfoes.FirstOrDefault(b => b.Eid == model.Eid);
                        if (basicInfo != null)
                        {
                            basicInfo.PersonalEmail = email;
                        }
                        else
                        {
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
                    return Json(new
                    {
                        success = false,
                        message = ex.InnerException?.Message ?? ex.Message
                    });
                }
            }
        }

        [HttpPost]
        public ActionResult Save( EEmployee model, string username, string password, int roleId, string email)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Check username
                    if (db.Logins.Any(x => x.UserName == username))
                        return Json(new { success = false, message = "Username already exists" });

                    // 2️⃣ Save Login FIRST (no FK dependencies anymore!)
                    var login = new Login
                    {
                        UserName = username,
                        Password = password,
                        RoleId = roleId,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    db.Logins.Add(login);
                    db.SaveChanges();
                    int loginId = login.LoginId;

                    // 3️⃣ Save EEmployee with LoginId
                    var employee = new EEmployee
                    {
                        LoginId = loginId,
                        FullName = model.FullName,
                        TypeId = Convert.ToInt32(model.TypeId),
                        Address = model.Address,
                        Mobile = model.Mobile,
                        Designation = model.Designation,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    db.EEmployees.Add(employee);
                    db.SaveChanges();

                    int empId = employee.Eid;

                    // 4️⃣ SAVE BASIC INFO
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var basicInfo = new BasicInfo
                        {
                            Eid = empId,
                            PersonalEmail = email,
                            CreatedDate = DateTime.Now
                        };

                        db.BasicInfoes.Add(basicInfo);
                        db.SaveChanges();
                    }

                    // 5️⃣ PERMISSIONS
                    var allTypes = db.EmployeeTypeMasters
                                     .Select(x => x.Id)
                                     .ToList();

                    bool isAdmin = (roleId == 1);

                    foreach (var typeId in allTypes)
                    {
                     db.UserRoleTypePermissions.Add(new UserRoleTypePermission
                     {
                            Eid = empId,
                            RoleId = roleId,
                            TypeId = typeId,
                            CanCreate = isAdmin,
                            CanRead = isAdmin,
                            CanUpdate = isAdmin,
                            CanDelete = isAdmin,
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        });
                    }

                    db.SaveChanges();

                    transaction.Commit();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    var error = ex.InnerException != null
                        ? ex.InnerException.InnerException?.Message ?? ex.InnerException.Message
                        : ex.Message;

                    return Json(new
                    {
                        success = false,
                        message = error
                    });
                }

            }
        }

        #endregion


        #region Profile View
        public ActionResult ProfileView(int? id = null)
        {
            EEmployee model;

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            model = db.EEmployees.FirstOrDefault(x => x.Eid == id.Value);
            if (model == null)
                return HttpNotFound();

            return View("Profile",model);
        }

        public ActionResult ProfileTab(String tab = null, int? id = null)
        {
            var data = db.EEmployees.FirstOrDefault(x => x.Eid == id); if (data == null) return HttpNotFound();
            if (id == null)
                return HttpNotFound();

            switch (tab)
            {
                case "BasicInfo":
                    var employee = db.EEmployees.FirstOrDefault(x => x.Eid == id);
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
                        EEmployee = employee,
                        BasicInfo = basicInfo
                    };
                    return PartialView("_BasicInfo", vm);


                case "EmploymentHistory":
                    employee = db.EEmployees.FirstOrDefault(x => x.Eid == id);
                    if (employee == null)
                        return HttpNotFound();

                    var EmpHistory = db.EmployeeEmploymentHistories
                        .Where(x => x.EmployeeId == id)
                        .ToList();
                    System.Diagnostics.Debug.WriteLine("Employee ID: " + id);

                    ViewBag.id = id;

                    var EHVM = new EmployeeVM
                    {
                        EEmployee = employee,
                        EmploymentHistory = EmpHistory
                    };
                    return PartialView("_EmploymentHistory", EHVM);


                case "Assessments":
                    return PartialView("_Assessments", data);


                case "EducationHistory":
                    employee = db.EEmployees.FirstOrDefault(x => x.Eid == id);
                    if (employee == null)
                        return HttpNotFound();

                    var EmpEduHistory = db.EmployeeEducationHistories
                        .Where(x => x.EmployeeId == id)
                        .ToList();

                    var EEHVM = new EmployeeVM
                    {
                        EEmployee = employee,
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

        #endregion Profile View

        #region Edit Basic Info

        [HttpGet]
        public ActionResult EditBasicInfo(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var employee = db.EEmployees.FirstOrDefault(x => x.Eid == id);
            if (employee == null)
                return HttpNotFound();

            var basicInfo = db.BasicInfoes.FirstOrDefault(x => x.Eid == id);

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
                EEmployee = employee,
                BasicInfo = basicInfo
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBasicInfo(EmployeeVM model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            try
            {
                // Update Employee
                var employee = db.EEmployees.Find(model.EEmployee.Eid);
                if (employee != null)
                {
                    employee.FullName = model.EEmployee.FullName;
                    employee.Mobile = model.EEmployee.Mobile;
                    employee.Address = model.EEmployee.Address;
                    employee.Designation = model.EEmployee.Designation;
                    employee.TypeId = model.EEmployee.TypeId;
                }

                // Update or Insert BasicInfo
                var basicInfo = db.BasicInfoes.FirstOrDefault(x => x.Eid == model.EEmployee.Eid);
                
                if (basicInfo == null)
                {
                    // Create new BasicInfo
                    basicInfo = new BasicInfo
                    {
                        Eid = model.EEmployee.Eid,
                        CreatedDate = DateTime.Now,
                        CreatedBy = Session["UserName"]?.ToString()
                    };
                    db.BasicInfoes.Add(basicInfo);
                }

                // Update BasicInfo fields
                basicInfo.DOB = model.BasicInfo.DOB;
                basicInfo.DOJ = model.BasicInfo.DOJ;
                basicInfo.Gender = model.BasicInfo.Gender;
                basicInfo.NickName = model.BasicInfo.NickName;
                basicInfo.PayType = model.BasicInfo.PayType;
                basicInfo.EmployeeType = model.BasicInfo.EmployeeType;
                basicInfo.Department = model.BasicInfo.Department;
                basicInfo.Designation = model.BasicInfo.Designation;
                basicInfo.PersonalEmail = model.BasicInfo.PersonalEmail;
                basicInfo.OfficeEmail = model.BasicInfo.OfficeEmail;
                basicInfo.HurricanePOCEmail = model.BasicInfo.HurricanePOCEmail;
                basicInfo.HurricanePOCPhone = model.BasicInfo.HurricanePOCPhone;
                basicInfo.ManagerName = model.BasicInfo.ManagerName;
                basicInfo.ZoneManagerId = model.BasicInfo.ZoneManagerId;
                basicInfo.CostCenter = model.BasicInfo.CostCenter;
                basicInfo.CostCenterAddress = model.BasicInfo.CostCenterAddress;
                basicInfo.HurricaneZone = model.BasicInfo.HurricaneZone;
                basicInfo.Competency = model.BasicInfo.Competency;
                basicInfo.LinkedInUrl = model.BasicInfo.LinkedInUrl;
                basicInfo.PaidOffDaysPerYear = model.BasicInfo.PaidOffDaysPerYear;
                basicInfo.Skills = model.BasicInfo.Skills;
                basicInfo.UpdatedDate = DateTime.Now;
                basicInfo.UpdatedBy = Session["UserName"]?.ToString();

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion Edit Basic Info

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
            
            var emp = db.EEmployees.FirstOrDefault(x => x.Eid == model.EmployeeId);
            model.CreatedBy = emp.FullName;
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

            var emp = db.EEmployees.FirstOrDefault(x => x.Eid == model.EmployeeId);
            if (emp != null)
            {
                history.UpdatedBy = emp.FullName;
            }
            history.UpdatedDate = DateTime.Now;

            db.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult SearchEmploymentHistory(int employeeId, string search)
        {
            var employee = db.EEmployees.FirstOrDefault(x => x.Eid == employeeId);
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
                EEmployee = employee,
                EmploymentHistory = history
                    .OrderByDescending(x => x.DateOfJoining) // ✅ keep sorting
                    .ToList()
            };

            return PartialView("_EmploymentHistoryRows", vm);
        }

        #endregion EmployeeHistory


        #region View Permissions

        [HttpGet]
        public ActionResult ViewPermissions(int employeeId)
        {
            var employee = db.EEmployees
                             .Include(e => e.Login)
                             .FirstOrDefault(e => e.Eid == employeeId);

            if (employee == null)
                return HttpNotFound();

            var permissions = (from t in db.EmployeeTypeMasters
                               join p in db.UserRoleTypePermissions
                                   on new { TypeId = t.Id, Eid = employeeId }
                                   equals new { p.TypeId, p.Eid }
                                   into tp
                               from p in tp.DefaultIfEmpty()
                               select new PermissionEditVM
                               {
                                   TypeId = t.Id,
                                   TypeName = t.Name,
                                   CanCreate = p != null && p.CanCreate,
                                   CanRead = p != null && p.CanRead,
                                   CanUpdate = p != null && p.CanUpdate,
                                   CanDelete = p != null && p.CanDelete
                               }).ToList();

            var vm = new ViewPermissionsVM
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FullName,
                RoleName = employee.Login.Role.Name,
                Permissions = permissions
            };

            return View(vm);
        }
      
        [HttpPost]
        public ActionResult UpdatePermissions(ViewPermissionsVM model)
        {
            foreach (var p in model.Permissions)
            {
                var entity = db.UserRoleTypePermissions
                    .FirstOrDefault(x => x.Eid == model.EmployeeId && x.TypeId == p.TypeId);

                if (entity == null)
                {
                    entity = new UserRoleTypePermission
                    {
                        Eid = model.EmployeeId,
                        TypeId = p.TypeId,
                        IsActive = true
                    };
                    db.UserRoleTypePermissions.Add(entity);
                }

                entity.CanCreate = p.CanCreate;
                entity.CanRead = p.CanRead;
                entity.CanUpdate = p.CanUpdate;
                entity.CanDelete = p.CanDelete;
            }

            db.SaveChanges();
            return Json(new { success = true });
        }

        #endregion View Permissions


        #region Manage Roles

        [HttpGet]
        public ActionResult ManageRoles()
        {
            var roleManagementData = db.UserRoleMenuViews
                .Join(
                    db.RolesTypesViews,
                    urmv => urmv.RoleTypeViewId,
                    rtv => rtv.Id,
                    (urmv, rtv) => new { urmv, rtv }
                )
                .Join(
                    db.Roles,
                    temp => temp.rtv.RoleId,
                    r => r.RoleId,
                    (temp, r) => new RoleManagementVM
                    {
                        Id = temp.urmv.Id,
                        RoleId = r.RoleId,
                        RoleName = r.Name,
                        RoleTypeViewId = temp.urmv.RoleTypeViewId,
                        ViewType = temp.urmv.ViewType,
                        IsActive = temp.urmv.IsActive,

                        FullTime = temp.rtv.FullTime,
                        PartTime = temp.rtv.PartTime,
                        Internship = temp.rtv.Internship,
                        Freelance = temp.rtv.Freelance,
                        Contract = temp.rtv.Contract,
                        Temporary = temp.rtv.Temporary
                    }
                )
                .ToList();

            return View(roleManagementData);
        }

        [HttpPost]
        public ActionResult UpdateRoleSettings(List<RoleManagementVM> roles)
        {
            try
            {
                foreach (var role in roles)
                {
                    // Update UserRoleMenuView
                    var menuView = db.UserRoleMenuViews.FirstOrDefault(x => x.Id == role.Id);
                    if (menuView != null)
                    {
                        menuView.ViewType = (byte)role.ViewType;
                        menuView.IsActive = role.IsActive;
                    }

                    // Update RolesTypesView
                    var roleTypeView = db.RolesTypesViews.FirstOrDefault(x => x.Id == role.RoleTypeViewId);
                    if (roleTypeView != null)
                    {
                        roleTypeView.FullTime = role.FullTime;
                        roleTypeView.PartTime = role.PartTime;
                        roleTypeView.Internship = role.Internship;
                        roleTypeView.Freelance = role.Freelance;
                        roleTypeView.Contract = role.Contract;
                        roleTypeView.Temporary = role.Temporary;
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Role settings updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion Manage Roles


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
