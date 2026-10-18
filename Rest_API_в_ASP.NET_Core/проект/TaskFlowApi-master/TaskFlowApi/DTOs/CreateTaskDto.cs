using System.ComponentModel.DataAnnotations;
using TaskFlowApi.Entities;

namespace TaskFlowApi.DTOs;

public class CreateTaskDto
{
    /// <summary>Заголовок задачи (5–200 символов)</summary>
    [Required, StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Описание задачи</summary>
    public string? Description { get; set; }

    /// <summary>Если не указан — ToDo</summary>
    public TaskItemStatus? Status { get; set; }

    /// <summary>Приоритет (только для v2)</summary>
    public TaskPriority? Priority { get; set; }

    /// <summary>ID проекта (обязательно)</summary>
    public int ProjectId { get; set; }

    /// <summary>ID исполнителя</summary>
    public int? AssignedToId { get; set; }

    /// <summary>Дата окончания (если указана — должна быть в будущем)</summary>
    public DateTime? DueDate { get; set; }
}