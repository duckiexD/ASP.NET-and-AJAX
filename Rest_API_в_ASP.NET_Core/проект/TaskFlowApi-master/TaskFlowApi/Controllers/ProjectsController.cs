using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Mapping;

namespace TaskFlowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProjectsController(AppDbContext db) => _db = db;

    /// <summary>Получить список всех проектов</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var items = await _db.Projects
            .Include(p => p.Tasks)
            .Select(p => p.ToDto())
            .ToListAsync();
        return Ok(items);
    }

    /// <summary>Получить проект по ID</summary>
    /// <param name="id">ID проекта</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var p = await _db.Projects.Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();
        return Ok(p.ToDto());
    }

    /// <summary>Создать проект</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto)
    {
        var p = new Project { Name = dto.Name, Description = dto.Description };
        _db.Projects.Add(p);
        await _db.SaveChangesAsync();

        var result = p.ToDto();
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, result);
    }

    /// <summary>Полностью обновить проект</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto)
    {
        var p = await _db.Projects.FindAsync(id);
        if (p is null) return NotFound();

        p.Name = dto.Name;
        p.Description = dto.Description;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Удалить проект (каскадно задачи и комментарии)</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Projects.FindAsync(id);
        if (p is null) return NotFound();

        _db.Projects.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}