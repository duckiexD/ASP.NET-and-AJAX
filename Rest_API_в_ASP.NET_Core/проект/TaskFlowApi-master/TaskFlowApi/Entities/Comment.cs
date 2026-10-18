using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Entities;

public class Comment
{
    public int Id { get; set; }

    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public int AuthorId { get; set; }
    public AppUser? Author { get; set; }

    [Required, StringLength(500, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}