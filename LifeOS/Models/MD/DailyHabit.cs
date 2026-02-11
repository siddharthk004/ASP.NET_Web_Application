using System;
using System.ComponentModel.DataAnnotations;

public class DailyHabit
{
    [Key]
    public int HabitId { get; set; }
    public int UserId { get; set; }

    public string HabitName { get; set; }
    public string TargetValue { get; set; }

    public DateTime HabitDate { get; set; }
    public bool IsCompleted { get; set; }

    public int Streak { get; set; }   // 🔥 important
}
