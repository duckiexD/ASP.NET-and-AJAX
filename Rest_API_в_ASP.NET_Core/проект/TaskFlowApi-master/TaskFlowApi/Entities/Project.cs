using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Entities;

/// <summary>Проект — контейнер для задач</summary>
public class Project
{
    public int Id { get; set; }

    [Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TaskItem> Tasks { get; set; } = new();
}