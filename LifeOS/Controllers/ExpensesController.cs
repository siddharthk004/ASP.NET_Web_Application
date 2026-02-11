using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LifeOS.Controllers
{
    public class ExpensesController : Controller
    {
        LifeOSContext db = new LifeOSContext();

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            
            var expenses = db.Expenses
                            .Where(e => e.UserId == userId && e.ExpenseDate >= startOfMonth)
                            .OrderByDescending(e => e.ExpenseDate)
                            .ToList();

            ViewBag.TotalExpenses = expenses.Sum(e => e.Amount);
            
            return View(expenses);
        }

        [HttpPost]
        public ActionResult Add(string reason, string category, decimal amount, DateTime? expenseDate)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];

            db.Expenses.Add(new Expense
            {
                UserId = userId,
                Reason = reason,
                Category = category,
                Amount = amount,
                ExpenseDate = expenseDate ?? DateTime.Now
            });

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var expense = db.Expenses.Find(id);
            if (expense != null)
            {
                db.Expenses.Remove(expense);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}