using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public DateTime? DueDate { get; set; }
    public string? AssigneeName { get; set; }
}
