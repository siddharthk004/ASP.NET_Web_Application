using System;
using System.Linq;
using System.Web.Mvc;

public class DashboardController : Controller
{
    LifeOSContext db = new LifeOSContext();

    public ActionResult Index()
    {
        if (Session["UserId"] == null)
            return RedirectToAction("Login", "Auth");

        int userId = (int)Session["UserId"];
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        // Get statistics
        ViewBag.UserName = Session["UserName"]?.ToString() ?? "User";
        ViewBag.TodayHabits = db.DailyHabits.Count(h => h.UserId == userId && h.HabitDate == today);
        ViewBag.CompletedHabits = db.DailyHabits.Count(h => h.UserId == userId && h.HabitDate == today && h.IsCompleted);
        
        ViewBag.PendingTasks = db.Tasks.Count(t => t.UserId == userId && t.Status == "Pending");
        //ViewBag.TodayTasks = db.Tasks.Count(t => t.UserId == userId && t.DueDateTime.Date == today);
        
        ViewBag.MonthlyExpenses = db.Expenses.Where(e => e.UserId == userId && e.ExpenseDate >= startOfMonth).Sum(e => (decimal?)e.Amount) ?? 0;
        
        ViewBag.TodayFocusMinutes = db.FocusLogs.Where(f => f.UserId == userId && f.LogDate >= today).Sum(f => (int?)f.MinutesSpent) ?? 0;
        
        var todayMood = db.MoodLogs.FirstOrDefault(m => m.UserId == userId && m.MoodDate == today);
        ViewBag.TodayMood = todayMood?.MoodLevel ?? 0;

        return View();
    }

    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Login", "Auth");
    }
}

