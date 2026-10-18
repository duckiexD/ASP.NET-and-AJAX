using System.ComponentModel.DataAnnotations;
using TaskFlowApi.Entities;

namespace TaskFlowApi.DTOs;

public class UpdateTaskDto
{
    /// <summary>Заголовок задачи (5–200 символов)</summary>
    [Required, StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Описание задачи</summary>
    public string? Description { get; set; }

    /// <summary>Обязательно при полной замене (PUT)</summary>
    [Required]
    public TaskItemStatus Status { get; set; }

    /// <summary>Приоритет</summary>
    public TaskPriority? Priority { get; set; }

    /// <summary>ID исполнителя</summary>
    public int? AssignedToId { get; set; }

    /// <summary>Дата окончания</summary>
    public DateTime? DueDate { get; set; }
}