using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TaskFlowApi.Entities;

public class AppUser
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public List<TaskItem> AssignedTasks { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}