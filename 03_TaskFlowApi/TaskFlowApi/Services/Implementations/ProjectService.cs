using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Services.Implementations;

public class ProjectService(IUnitOfWork uow) : IProjectService
{
    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await uow.Projects.GetAllWithDetailsAsync();
        return projects.Select(Map).ToList();
    }

    public async Task<ProjectDto> GetByIdAsync(int id)
    {
        var project = await uow.Projects.GetByIdWithDetailsAsync(id)
            ?? throw new ApiException(404, $"Проект #{id} не найден.");
        return Map(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, int ownerId)
    {
        var owner = await uow.Users.GetByIdAsync(ownerId)
            ?? throw new ApiException(404, "Владелец проекта не найден.");

        var project = new Project
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            Status = dto.Status,
            OwnerId = ownerId,
            Owner = owner
        };
        await uow.Projects.AddAsync(project);
        await uow.SaveChangesAsync();
        return Map(project);
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto, int userId, bool isAdmin)
    {
        var project = await uow.Projects.GetByIdWithDetailsAsync(id)
            ?? throw new ApiException(404, $"Проект #{id} не найден.");
        if (!isAdmin && project.OwnerId != userId)
            throw new ApiException(403, "Редактировать проект может только его владелец или администратор.");

        project.Name = dto.Name.Trim();
        project.Description = dto.Description.Trim();
        project.Status = dto.Status;
        uow.Projects.Update(project);
        await uow.SaveChangesAsync();
        return Map(project);
    }

    public async Task DeleteAsync(int id, int userId, bool isAdmin)
    {
        var project = await uow.Projects.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Проект #{id} не найден.");
        if (!isAdmin && project.OwnerId != userId)
            throw new ApiException(403, "Удалить проект может только его владелец или администратор.");

        uow.Projects.Remove(project);
        await uow.SaveChangesAsync();
    }

    private static ProjectDto Map(Project p) => new(
        p.Id, p.Name, p.Description, p.Status, p.CreatedAtUtc,
        p.OwnerId, p.Owner?.Username ?? "unknown", p.Tasks.Count);
}
