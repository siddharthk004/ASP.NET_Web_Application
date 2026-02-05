using System;

namespace EmployeeCRUD.Models
{
    public class UserRoleTypePermissions
    {
        public int Id { get; set; }
        public int Eid { get; set; }
        public int RoleId { get; set; }
        public int TypeId { get; set; }

        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
