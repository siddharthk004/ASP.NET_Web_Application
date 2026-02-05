using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeCRUD.Models
{
    public class TypePermissionDto
    {
        public int TypeId { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}