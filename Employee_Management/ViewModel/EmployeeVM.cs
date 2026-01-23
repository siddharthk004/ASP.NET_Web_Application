using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee_Management.ViewModel
{
    public class EmployeeVM
    {
        // Employee
        public int EId{ get; set; }

        // Address Selection
        public bool UseExistingAddress { get; set; }
        public int? SelectedAddressId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Pincode { get; set; }

        // Role Selection
        public bool UseExistingRole { get; set; }
        public int? SelectedRoleId { get; set; }
        public string Role{ get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        // Dropdown Lists
        public IEnumerable<SelectListItem> AddressList { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}