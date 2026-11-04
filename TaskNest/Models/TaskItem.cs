using System.ComponentModel.DataAnnotations;

namespace TaskNest.Models;

public sealed class TaskItem
{
    public int Id { get; set; }
    [Required, StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = "";
    [StringLength(500)]
    public string Description { get; set; } = "";
    [Required]
    public string Category { get; set; } = "General";
    [Required]
    public string Priority { get; set; } = "Medium";
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
