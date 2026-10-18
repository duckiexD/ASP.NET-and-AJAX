using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Mapping;

namespace TaskFlowApi.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/tasks")]
[Produces("application/json")]
public class TasksV2Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksV2Controller(AppDbContext db) => _db = db;

    /// <summary>Список всех задач (v2, с Priority, без Description)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TaskV2Dto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TaskV2Dto>>> GetAll()
    {
        var list = await _db.Tasks.Include(t => t.AssignedTo)
            .Select(t => t.ToV2Dto()).ToListAsync();
        return Ok(list);
    }

    /// <summary>Получить задачу по ID (v2)</summary>
    /// <param name="id">ID задачи</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskV2Dto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskV2Dto>> GetById(int id)
    {
        var t = await _db.Tasks.Include(x => x.AssignedTo)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (t is null) return NotFound();
        return Ok(t.ToV2Dto());
    }

    /// <summary>Создать задачу (v2, с Priority)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskV2Dto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskV2Dto>> Create([FromBody] CreateTaskDto dto)
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

        return CreatedAtAction(nameof(GetById), new { id = t.Id, version = "2" }, t.ToV2Dto());
    }
}