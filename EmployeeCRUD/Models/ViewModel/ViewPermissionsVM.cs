using System;
using System.Collections.Generic;

namespace EmployeeCRUD.Models
{
    public class ViewPermissionsVM
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string RoleName { get; set; }
        public List<PermissionEditVM> Permissions { get; set; }
    }

}