using System.Linq;
using System.Net;
using System.Web.Mvc;
using Emp_Task_Management_System.Models;

namespace Emp_Task_Management_System.Controllers
{
    public class CredentialController : Controller
    {
        private readonly ETMSEntities db = new ETMSEntities();


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
