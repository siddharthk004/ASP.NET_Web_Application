using System;
using System.ComponentModel.DataAnnotations;

public class FocusLog
{
    [Key]
    public int FocusId { get; set; }
    public int UserId { get; set; }

    public string Category { get; set; }   // Coding, Study, Social
    public int MinutesSpent { get; set; }

    public DateTime LogDate { get; set; }
}
