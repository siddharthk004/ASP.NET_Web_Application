using System;
using System.Collections.Generic;

namespace EmployeeCRUD.Models
{
    public class EmployeeVM
    {
        public EEmployee EEmployee { get; set; }
        public BasicInfo BasicInfo { get; set; }
        public List<EmployeeEmploymentHistory> EmploymentHistory { get; set; }
        public List<EmployeeEducationHistory> EducationHistory { get; set; }

        public EmployeeVM()
        {
            EmploymentHistory = new List<EmployeeEmploymentHistory>();
            EducationHistory = new List<EmployeeEducationHistory>();
        }
    }
}