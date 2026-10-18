using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Mapping;

public static class MappingExtensions
{
    public static ProjectDto ToDto(this Project p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        CreatedAt = p.CreatedAt,
        TaskCount = p.Tasks?.Count ?? 0
    };

    public static TaskItemDto ToDto(this TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status,
        ProjectId = t.ProjectId,
        AssignedToId = t.AssignedToId,
        AssignedToUsername = t.AssignedTo?.Username,
        DueDate = t.DueDate
    };

    public static TaskV2Dto ToV2Dto(this TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Status = t.Status,
        Priority = t.Priority,
        ProjectId = t.ProjectId,
        AssignedToId = t.AssignedToId,
        AssignedToUsername = t.AssignedTo?.Username,
        DueDate = t.DueDate
    };

    public static CommentDto ToDto(this Comment c) => new()
    {
        Id = c.Id,
        Content = c.Content,
        AuthorUsername = c.Author?.Username ?? "unknown",
        CreatedAt = c.CreatedAt
    };
}