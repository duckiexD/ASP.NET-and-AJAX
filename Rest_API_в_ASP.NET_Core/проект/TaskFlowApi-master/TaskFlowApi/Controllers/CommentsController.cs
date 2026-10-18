using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Mapping;

namespace TaskFlowApi.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CommentsController(AppDbContext db) => _db = db;

    /// <summary>Список комментариев задачи</summary>
    /// <param name="taskId">ID задачи</param>
    [HttpGet("tasks/{taskId:int}/comments")]
    [ProducesResponseType(typeof(List<CommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<CommentDto>>> GetForTask(int taskId)
    {
        var exists = await _db.Tasks.AnyAsync(t => t.Id == taskId);
        if (!exists) return NotFound(new ProblemDetails
        {
            Title = "Task not found",
            Status = 404,
            Detail = $"Задача {taskId} не найдена"
        });

        var list = await _db.Comments
            .Where(c => c.TaskItemId == taskId)
            .Include(c => c.Author)
            .Select(c => c.ToDto())
            .ToListAsync();
        return Ok(list);
    }

    /// <summary>Добавить комментарий к задаче</summary>
    /// <param name="taskId">ID задачи</param>
    [HttpPost("tasks/{taskId:int}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> Create(int taskId, [FromBody] CreateCommentDto dto)
    {
        var task = await _db.Tasks.FindAsync(taskId);
        if (task is null) return NotFound(new ProblemDetails
        {
            Title = "Task not found",
            Status = 404,
            Detail = $"Задача {taskId} не найдена"
        });

        var author = await _db.Users.FindAsync(dto.AuthorId);
        if (author is null) return NotFound(new ProblemDetails
        {
            Title = "Author not found",
            Status = 404,
            Detail = $"Пользователь {dto.AuthorId} не найден"
        });

        var c = new Comment
        {
            TaskItemId = taskId,
            AuthorId = dto.AuthorId,
            Content = dto.Content
        };
        _db.Comments.Add(c);
        await _db.SaveChangesAsync();
        await _db.Entry(c).Reference(x => x.Author).LoadAsync();

        return CreatedAtAction(nameof(GetForTask), new { taskId }, c.ToDto());
    }

    /// <summary>Удалить комментарий</summary>
    /// <param name="id">ID комментария</param>
    [HttpDelete("comments/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Comments.FindAsync(id);
        if (c is null) return NotFound();

        _db.Comments.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}