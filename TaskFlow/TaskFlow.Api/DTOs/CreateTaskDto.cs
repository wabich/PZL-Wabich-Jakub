using System.ComponentModel.DataAnnotations;
using TaskFlow.Api.Domain.Enums;

namespace TaskFlow.Api.DTOs;
public class CreateTaskDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }

    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }

    public int? AssigneeId { get; set; }
}