using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.DTOs;

public class CreateProjectDto
{
    /// <summary>Название проекта (3–100 символов)</summary>
    [Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Описание проекта (до 500 символов)</summary>
    [StringLength(500)]
    public string? Description { get; set; }
}