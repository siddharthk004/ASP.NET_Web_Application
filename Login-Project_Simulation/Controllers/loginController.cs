using Login_Project_Simulation.DateTimeDTO;
using Login_Project_Simulation.loginDTO;
using Login_Project_Simulation.models;
using Login_Project_Simulation.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace Login_Project_Simulation.Controllers
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private const int V = 401;
        private readonly SanmolEntities db = new SanmolEntities();


        [HttpPost]
        [Route("userlogin")]
        public IHttpActionResult Login(LoginDTOc model)
        {
            if(model == null)
            {
                return Unauthorized();
            }
            var user = db.logins.FirstOrDefault(u => u.name == model.Username && u.password == model.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(new
            {
                message = "Login Success",
                luser = model.Username,
                lpassword = model.Password
            });
        }

        [HttpPost]
        [Route("userlogin/cheack")]
        public IHttpActionResult Loginchk(LoginDTOc model)
        {
            if (model == null)
            {
                return Unauthorized();
            }
            var user = db.logins.FirstOrDefault(u => u.name == model.Username && u.password == model.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(new
            {
                message = "Login Success",
                luser = model.Username,
                lpassword = model.Password
            });
        }
        
        [HttpPost]
        [Route("exception")]
        public IHttpActionResult Exception(string username)
        {
            if(username == null)
            {
                   return Unauthorized();
            }
            var exuser = db.logins.FirstOrDefault(u => u.name == username);
            try
            {
                if (exuser == null)
                {
                    throw new Models.NullReferenceException("User not found");
                }
            }
            catch (Models.NullReferenceException ex)
            {
                return Content((System.Net.HttpStatusCode)V, new
                {
                    message = ex.Message
                });
            }
            return Ok("Login Successfull");
        }

        [HttpGetAttribute]
        [Route("cust/list")]
        public IHttpActionResult CustList(int page = 1,int pageSize = 2)
        {
            var emp = db.Employees.AsNoTracking()
                .OrderBy(c => c.Eid)
               .Skip((page - 1) * pageSize)
               .Take(pageSize).ToList();
            return Ok(emp);
        }

        [HttpGetAttribute]
        //[Authorize(Roles = "Admin")]
        [Route("get/user/all")]
        public IHttpActionResult GetAllUser(String role)
        {
            if(role == "Admin")
            {
                var alluser = db.Employees.ToList();
                return Ok(alluser);
            }
            return StatusCode(HttpStatusCode.Forbidden);
        }

        [HttpGetAttribute]
        [Route("Search")]
        public IHttpActionResult Search(String query,int page = 1,int pageSize = 3)
        {
            query = query?.ToLower() ?? "";
            var querys = db.Employees.AsNoTracking().Where(e => e.IsActive);
            if(!string.IsNullOrEmpty(query))
            {
                querys = querys.Where(e =>
                e.Ename.ToLower().Contains(query) || e.Emob.ToLower().Contains(query));
            }
            int total = querys.Count();

            var emp = querys
                .OrderBy(c => c.Eid)
               .Skip((page - 1) * pageSize)
               .Take(pageSize).ToList();

            return Ok(new
            {
                TotalRecord = total,
                pages = page,
                pageSizes = pageSize,
                Data = emp
            });
        }

        [HttpGetAttribute]
        [Route("dlk")]
        public IHttpActionResult Deadlock()
        {
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var emp = db.Employees.Select(e => e).FirstOrDefault();

                    emp.Emob = "9865969305";
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return Ok();
        }

        [HttpPost]
        [Route("valid")]
        public IHttpActionResult Validation()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok("Success");
        }

        [HttpGetAttribute]
        [Route("get/user/dto")]
        public IHttpActionResult GetAllUserdto()
        {
            var alluserdto = db.Employees.AsNoTracking().Select(u => new UserResponseDTOs
            {
                ID = u.Eid,
                Name = u.Ename,
                Type = u.Etype,
                Design = u.Edesign,
                Addr = u.Eaddr
            }).ToList();
            return Ok(alluserdto);
        }

        [HttpPost]
        [Route("date")]
        public IHttpActionResult PostDate(DateTimeDTOx model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("Date", "YYYY-MM-DD");
                    return BadRequest(ModelState);
                }
                return Ok(new
                {
                    message = "Date received",
                    datea = model.Date
                });
            }
            catch(Exception e)
            {
                ModelState.AddModelError("Date", "YYYY-MM-DD");
                Console.WriteLine(e);
                return BadRequest(ModelState);
            }
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