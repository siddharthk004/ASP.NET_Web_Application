using System.Linq;
using System.Web.Mvc;

public class AuthController : Controller
{
    LifeOSContext db = new LifeOSContext();

    // GET: Login
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string email, string password)
    {
        var user = db.Users.FirstOrDefault(x => x.Email == email && x.PasswordHash == password);

        if (user != null)
        {
            Session["UserId"] = user.UserId;
            Session["UserName"] = user.Name;

            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Error = "Invalid email or password";
        return View();
    }

    // GET: Register
    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Register(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "All fields are required";
            return View();
        }

        // Check if user already exists
        if (db.Users.Any(u => u.Email == email))
        {
            ViewBag.Error = "Email already registered";
            return View();
        }

        var user = new User
        {
            Name = name,
            Email = email,
            PasswordHash = password,
            CreatedAt = System.DateTime.Now
        };

        db.Users.Add(user);
        db.SaveChanges();

        Session["UserId"] = user.UserId;
        Session["UserName"] = user.Name;

        return RedirectToAction("Index", "Dashboard");
    }

    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Login");
    }
}
