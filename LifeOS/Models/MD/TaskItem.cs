using System;
using System.ComponentModel.DataAnnotations;

public class TaskItem
{
    [Key]
    public int TaskId { get; set; }
    public int UserId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public DateTime DueDateTime { get; set; }
    public string Priority { get; set; }   // Low / Medium / High
    public string Status { get; set; }     // Pending / Done / Missed

    public DateTime CreatedAt { get; set; }
}
