using TaskFlowApi.Entities;

namespace TaskFlowApi.DTOs;

/// <summary>Задача в API v2: есть Priority, нет Description</summary>
public class TaskV2Dto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }
    public int? AssignedToId { get; set; }
    public string? AssignedToUsername { get; set; }
    public DateTime? DueDate { get; set; }
}