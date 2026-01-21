
using Emp_Task_Management_System.Models;
using System.Collections.Generic;

namespace Emp_Task_Management_System.Controllers
{
    public class DashboardViewModelc
    {
        public IEnumerable<Registration> Registrations { get; set; }
        public IEnumerable<Task> Tasks { get; set; }
    }

}
