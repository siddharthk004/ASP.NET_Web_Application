using System;
using System.Linq;
using System.Web.Mvc;

namespace LifeOS.Controllers
{
    public class IncomeController : Controller
    {
        LifeOSContext db = new LifeOSContext();

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];
            var incomes = db.Incomes
                           .Where(i => i.UserId == userId)
                           .OrderByDescending(i => i.Year)
                           .ThenByDescending(i => i.Month)
                           .ToList();

            return View(incomes);
        }

        [HttpPost]
        public ActionResult Add(int month, int year, decimal monthlyIncome)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];

            var existing = db.Incomes.FirstOrDefault(i => i.UserId == userId && i.Month == month && i.Year == year);

            if (existing != null)
            {
                existing.MonthlyIncome = monthlyIncome;
            }
            else
            {
                db.Incomes.Add(new Income
                {
                    UserId = userId,
                    Month = month,
                    Year = year,
                    MonthlyIncome = monthlyIncome
                });
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var income = db.Incomes.Find(id);
            if (income != null)
            {
                db.Incomes.Remove(income);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
