using System;
using System.Collections.Generic;

namespace EmployeeCRUD.Models
{
    public class RoleManagementVM
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int RoleTypeViewId { get; set; }
        public int ViewType { get; set; }
        public bool IsActive { get; set; }

        // Role Type Flags
        public bool FullTime { get; set; }
        public bool PartTime { get; set; }
        public bool Internship { get; set; }
        public bool Freelance { get; set; }
        public bool Contract { get; set; }
        public bool Temporary { get; set; }
    }
}
