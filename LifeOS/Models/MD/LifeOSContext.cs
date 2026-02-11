using System.Data.Entity;

public class LifeOSContext : DbContext
{
    public LifeOSContext() : base("LifeOSConnection") { }

    public DbSet<User> Users { get; set; }
    public DbSet<DailyHabit> DailyHabits { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<FocusLog> FocusLogs { get; set; }
    public DbSet<MoodLog> MoodLogs { get; set; }
}
