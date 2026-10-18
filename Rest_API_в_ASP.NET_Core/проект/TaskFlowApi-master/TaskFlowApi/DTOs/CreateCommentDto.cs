using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.DTOs;

public class CreateCommentDto
{
    /// <summary>Текст комментария (1–500 символов)</summary>
    [Required, StringLength(500, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;

    /// <summary>ID автора комментария</summary>
    public int AuthorId { get; set; }
}