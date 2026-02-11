using System;
using System.ComponentModel.DataAnnotations;

public class MoodLog
{
    [Key]
    public int MoodId { get; set; }
    public int UserId { get; set; }

    public int MoodLevel { get; set; } // 1–5
    public DateTime MoodDate { get; set; }
}
