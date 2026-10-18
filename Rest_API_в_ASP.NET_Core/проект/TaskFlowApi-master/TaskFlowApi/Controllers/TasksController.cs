using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Mapping;

namespace TaskFlowApi.Controllers;

[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksController(AppDbContext db) => _db = db;

    /// <summary>Список всех задач (v1)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TaskItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TaskItemDto>>> GetAll()
    {
        var list = await _db.Tasks.Include(t => t.AssignedTo)
            .Select(t => t.ToDto()).ToListAsync();
        return Ok(list);
    }

    /// <summary>Получить задачу по ID</summary>
    /// <param name="id">ID задачи</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> GetById(int id)
    {
        var t = await _db.Tasks.Include(x => x.AssignedTo)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (t is null) return NotFound();
        return Ok(t.ToDto());
    }

    /// <summary>Создать задачу. Status по умолчанию — ToDo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskItemDto>> Create([FromBody] CreateTaskDto dto)
    {
        if (dto.DueDate.HasValue && dto.DueDate.Value <= DateTime.UtcNow)
            return BadRequest(new ProblemDetails { Title = "DueDate must be in the future" });

        var project = await _db.Projects.FindAsync(dto.ProjectId);
        if (project is null)
            return BadRequest(new ProblemDetails { Title = $"Project {dto.ProjectId} not found" });

        var t = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status ?? TaskItemStatus.ToDo,
            Priority = dto.Priority ?? TaskPriority.Medium,
            ProjectId = dto.ProjectId,
            AssignedToId = dto.AssignedToId,
            DueDate = dto.DueDate
        };
        _db.Tasks.Add(t);
        await _db.SaveChangesAsync();
        await _db.Entry(t).Reference(x => x.AssignedTo).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = t.Id, version = "1" }, t.ToDto());
    }

    /// <summary>Полностью обновить задачу. Status обязателен.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var t = await _db.Tasks.FindAsync(id);
        if (t is null) return NotFound();

        t.Title = dto.Title;
        t.Description = dto.Description;
        t.Status = dto.Status;
        if (dto.Priority.HasValue) t.Priority = dto.Priority.Value;
        t.AssignedToId = dto.AssignedToId;
        t.DueDate = dto.DueDate;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Удалить задачу</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Tasks.FindAsync(id);
        if (t is null) return NotFound();

        _db.Tasks.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}