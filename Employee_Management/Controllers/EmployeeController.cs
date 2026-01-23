using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Employee_Management.Models;
using Employee_Management.ViewModel;

namespace Employee_Management.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ETMSEntities db = new ETMSEntities();

        // GET: Employee/Create
        public ActionResult Create()
        {
            var model = new EmployeeVM
            {
                AddressList = db.EmpAddresses.AsEnumerable().Select(a => new SelectListItem
                {
                    Value = a.AddrId.ToString(),
                    Text = a.City + ", " + a.State + " - " + a.Pincode
                }).ToList(),
                RoleList = db.EmpRoles.AsEnumerable().Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.Role + " (" + r.FromDate.ToString("dd/MM/yyyy") + " - " + r.ToDate.ToString("dd/MM/yyyy") + ")"
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EmployeeVM model)
        {
            if (!ModelState.IsValid)
            {
                model.AddressList = db.EmpAddresses.AsEnumerable().Select(a => new SelectListItem
                {
                    Value = a.AddrId.ToString(),
                    Text = a.City + ", " + a.State + " - " + a.Pincode
                }).ToList();
                model.RoleList = db.EmpRoles.AsEnumerable().Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.Role + " (" + r.FromDate.ToString("dd/MM/yyyy") + " - " + r.ToDate.ToString("dd/MM/yyyy") + ")"
                }).ToList();
                return View(model);
            }

            int addressId;
            int roleId;

            // Handle Address
            if (model.UseExistingAddress && model.SelectedAddressId.HasValue)
            {
                addressId = model.SelectedAddressId.Value;
            }
            else
            {
                var address = new EmpAddress
                {
                    City = model.City,
                    State = model.State,
                    Pincode = model.Pincode,
                    CreatedAt = DateTime.Now
                };
                db.EmpAddresses.Add(address);
                db.SaveChanges();
                addressId = address.AddrId;
            }

            // Handle Role
            if (model.UseExistingRole && model.SelectedRoleId.HasValue)
            {
                roleId = model.SelectedRoleId.Value;
            }
            else
            {
                var role = new EmpRole
                {
                    Role = model.Role,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedAt = DateTime.Now
                };
                db.EmpRoles.Add(role);
                db.SaveChanges();
                roleId = role.RoleId;
            }

            var emp = new Employee
            {
                EId = model.EId,
                EAddrId = addressId,
                ERoleId = roleId,
                CreatedAt = DateTime.Now
            };
            db.Employees.Add(emp);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Employee
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.EmpAddress).Include(e => e.EmpRole);
            return View(employees.ToList());
        }

        // GET: Employee/Edit - List all employees for editing
        public ActionResult Edit()
        {
            var employees = db.Employees.Include(e => e.EmpAddress).Include(e => e.EmpRole).ToList();
            return View(employees);
        }

        // GET: Employee/EditForm/5 - Show edit form for specific employee
        public ActionResult EditForm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Employee employee = db.Employees.Include(e => e.EmpAddress).Include(e => e.EmpRole).FirstOrDefault(e => e.EId == id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            var model = new EmployeeVM
            {
                EId = employee.EId,
                
                // Current Address Info
                City = employee.EmpAddress?.City,
                State = employee.EmpAddress?.State,
                Pincode = employee.EmpAddress?.Pincode ?? 0,
                SelectedAddressId = employee.EAddrId,
                
                // Current Role Info
                Role = employee.EmpRole?.Role,
                FromDate = employee.EmpRole?.FromDate ?? DateTime.Now,
                ToDate = employee.EmpRole?.ToDate ?? DateTime.Now,
                SelectedRoleId = employee.ERoleId,
                
                // Dropdown Lists
                AddressList = db.EmpAddresses.AsEnumerable().Select(a => new SelectListItem
                {
                    Value = a.AddrId.ToString(),
                    Text = a.City + ", " + a.State + " - " + a.Pincode
                }).ToList(),
                RoleList = db.EmpRoles.AsEnumerable().Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.Role + " (" + r.FromDate.ToString("dd/MM/yyyy") + " - " + r.ToDate.ToString("dd/MM/yyyy") + ")"
                }).ToList()
            };

            return View(model);
        }

        // POST: Employee/EditForm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForm(EmployeeVM model)
        {
            if (!ModelState.IsValid)
            {
                model.AddressList = db.EmpAddresses.AsEnumerable().Select(a => new SelectListItem
                {
                    Value = a.AddrId.ToString(),
                    Text = a.City + ", " + a.State + " - " + a.Pincode
                }).ToList();
                model.RoleList = db.EmpRoles.AsEnumerable().Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.Role + " (" + r.FromDate.ToString("dd/MM/yyyy") + " - " + r.ToDate.ToString("dd/MM/yyyy") + ")"
                }).ToList();
                return View(model);
            }

            var employee = db.Employees.Include(e => e.EmpAddress).Include(e => e.EmpRole).FirstOrDefault(e => e.EId == model.EId);
            if (employee == null)
            {
                return HttpNotFound();
            }

            int addressId;
            int roleId;

            // Handle Address
            if (model.UseExistingAddress && model.SelectedAddressId.HasValue)
            {
                // Use selected existing address
                addressId = model.SelectedAddressId.Value;
                
                // Delete old address if it's different
                if (employee.EAddrId != addressId && employee.EmpAddress != null)
                {
                    db.EmpAddresses.Remove(employee.EmpAddress);
                }
            }
            else
            {
                // Update existing address or create new one
                if (employee.EmpAddress != null)
                {
                    // Update current address
                    employee.EmpAddress.City = model.City;
                    employee.EmpAddress.State = model.State;
                    employee.EmpAddress.Pincode = model.Pincode;
                    addressId = employee.EAddrId;
                }
                else
                {
                    // Create new address
                    var address = new EmpAddress
                    {
                        City = model.City,
                        State = model.State,
                        Pincode = model.Pincode,
                        CreatedAt = DateTime.Now
                    };
                    db.EmpAddresses.Add(address);
                    db.SaveChanges();
                    addressId = address.AddrId;
                }
            }

            // Handle Role
            if (model.UseExistingRole && model.SelectedRoleId.HasValue)
            {
                // Use selected existing role
                roleId = model.SelectedRoleId.Value;
                
                // Delete old role if it's different
                if (employee.ERoleId != roleId && employee.EmpRole != null)
                {
                    db.EmpRoles.Remove(employee.EmpRole);
                }
            }
            else
            {
                // Update existing role or create new one
                if (employee.EmpRole != null)
                {
                    // Update current role
                    employee.EmpRole.Role = model.Role;
                    employee.EmpRole.FromDate = model.FromDate;
                    employee.EmpRole.ToDate = model.ToDate;
                    roleId = employee.ERoleId;
                }
                else
                {
                    // Create new role
                    var role = new EmpRole
                    {
                        Role = model.Role,
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        CreatedAt = DateTime.Now
                    };
                    db.EmpRoles.Add(role);
                    db.SaveChanges();
                    roleId = role.RoleId;
                }
            }

            // Update employee references
            employee.EAddrId = addressId;
            employee.ERoleId = roleId;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Employee/Delete - List all employees for deletion
        public ActionResult Delete()
        {
            var employees = db.Employees.Include(e => e.EmpAddress).Include(e => e.EmpRole).ToList();
            return View(employees);
        }

        // GET: Employee/DeleteConfirm/5 - Confirm deletion of specific employee
        public ActionResult DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Include(e => e.EmpAddress).Include(e => e.EmpRole).FirstOrDefault(e => e.EId == id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employee/DeleteConfirm/5
        [HttpPost, ActionName("DeleteConfirm")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Include(e => e.EmpAddress).Include(e => e.EmpRole).FirstOrDefault(e => e.EId == id);
            
            if (employee == null)
            {
                return HttpNotFound();
            }

            // Store references before deleting employee
            var address = employee.EmpAddress;
            var role = employee.EmpRole;

            // Delete employee first
            db.Employees.Remove(employee);

            // Delete associated address if it exists
            if (address != null)
            {
                db.EmpAddresses.Remove(address);
            }

            // Delete associated role if it exists
            if (role != null)
            {
                db.EmpRoles.Remove(role);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
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

