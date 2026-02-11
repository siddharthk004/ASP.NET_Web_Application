using System;
using System.Linq;
using System.Web.Mvc;

public class HabitsController : Controller
{
    LifeOSContext db = new LifeOSContext();

    // Show today's habits
    public ActionResult Index()
    {
        if (Session["UserId"] == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        
        int userId = (int)Session["UserId"];
        DateTime today = DateTime.Today;

        var habits = db.DailyHabits
                       .Where(h => h.UserId == userId && h.HabitDate == today)
                       .ToList();

        return View(habits);
    }

    // Add new habit
    [HttpPost]
    public ActionResult Add(string habitName, string targetValue)
    {
        if (Session["UserId"] == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        
        int userId = (int)Session["UserId"];

        db.DailyHabits.Add(new DailyHabit
        {
            UserId = userId,
            HabitName = habitName,
            TargetValue = targetValue,
            HabitDate = DateTime.Today,
            IsCompleted = false,
            Streak = 0
        });

        db.SaveChanges();
        return RedirectToAction("Index");
    }

    // Mark habit done
    public ActionResult Complete(int id)
    {
        if (Session["UserId"] == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        
        var habit = db.DailyHabits.Find(id);

        if (habit != null && !habit.IsCompleted)
        {
            habit.IsCompleted = true;

            // 🔥 Streak logic
            var yesterday = db.DailyHabits.FirstOrDefault(h =>
                h.UserId == habit.UserId &&
                h.HabitName == habit.HabitName &&
                h.HabitDate == DateTime.Today.AddDays(-1) &&
                h.IsCompleted);

            habit.Streak = yesterday != null ? yesterday.Streak + 1 : 1;

            db.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}
