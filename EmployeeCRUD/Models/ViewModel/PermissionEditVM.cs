using System;
using System.Collections.Generic;

namespace EmployeeCRUD.Models
{
    public class PermissionEditVM
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

}