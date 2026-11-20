using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Services.Implementations;

public class TaskItemService(IUnitOfWork uow) : ITaskItemService
{
    public async Task<List<TaskItemDto>> GetAllAsync(int? projectId = null, string? status = null, int? tagId = null)
    {
        var tasks = await uow.Tasks.GetAllWithDetailsAsync(projectId, status, tagId);
        return tasks.Select(Map).ToList();
    }

    public async Task<TaskItemDto> GetByIdAsync(int id)
    {
        var task = await uow.Tasks.GetByIdWithDetailsAsync(id)
            ?? throw new ApiException(404, $"Задача #{id} не найдена.");
        return Map(task);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto)
    {
        var project = await uow.Projects.GetByIdAsync(dto.ProjectId)
            ?? throw new ApiException(404, $"Проект #{dto.ProjectId} не найден.");

        User? assignee = null;
        if (dto.AssigneeId.HasValue)
            assignee = await uow.Users.GetByIdAsync(dto.AssigneeId.Value)
                ?? throw new ApiException(404, $"Исполнитель #{dto.AssigneeId} не найден.");

        var tags = dto.TagIds is { Count: > 0 } ? await uow.Tags.GetByIdsAsync(dto.TagIds) : [];

        var task = new TaskItem
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Todo" : dto.Status,
            Priority = string.IsNullOrWhiteSpace(dto.Priority) ? "Medium" : dto.Priority,
            DueDateUtc = dto.DueDateUtc?.ToUniversalTime(),
            ProjectId = project.Id,
            Project = project,
            AssigneeId = assignee?.Id,
            Assignee = assignee,
            Tags = tags
        };

        await uow.Tasks.AddAsync(task);
        await uow.SaveChangesAsync();
        return Map(task);
    }

    public async Task<TaskItemDto> UpdateAsync(int id, UpdateTaskItemDto dto)
    {
        var task = await uow.Tasks.GetByIdWithDetailsAsync(id)
            ?? throw new ApiException(404, $"Задача #{id} не найдена.");
        var project = await uow.Projects.GetByIdAsync(dto.ProjectId)
            ?? throw new ApiException(404, $"Проект #{dto.ProjectId} не найден.");

        User? assignee = null;
        if (dto.AssigneeId.HasValue)
            assignee = await uow.Users.GetByIdAsync(dto.AssigneeId.Value)
                ?? throw new ApiException(404, $"Исполнитель #{dto.AssigneeId} не найден.");

        task.Title = dto.Title.Trim();
        task.Description = dto.Description?.Trim() ?? string.Empty;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDateUtc = dto.DueDateUtc?.ToUniversalTime();
        task.ProjectId = project.Id;
        task.Project = project;
        task.AssigneeId = assignee?.Id;
        task.Assignee = assignee;

        task.Tags.Clear();
        if (dto.TagIds is { Count: > 0 })
            foreach (var tag in await uow.Tags.GetByIdsAsync(dto.TagIds))
                task.Tags.Add(tag);

        uow.Tasks.Update(task);
        await uow.SaveChangesAsync();
        return Map(task);
    }

    public async Task DeleteAsync(int id)
    {
        var task = await uow.Tasks.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Задача #{id} не найдена.");
        uow.Tasks.Remove(task);
        await uow.SaveChangesAsync();
    }

    private static TaskItemDto Map(TaskItem t) => new(
        t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDateUtc, t.CreatedAtUtc,
        t.ProjectId, t.Project?.Name ?? string.Empty,
        t.AssigneeId, t.Assignee?.Username, t.Comments.Count,
        t.Tags.Select(tag => new TagDto(tag.Id, tag.Name, tag.Color)).ToList());
}
