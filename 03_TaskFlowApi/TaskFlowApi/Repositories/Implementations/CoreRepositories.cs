using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Entities;
using TaskFlowApi.Repositories.Interfaces;

namespace TaskFlowApi.Repositories.Implementations;

public class UserRepository(TaskFlowDbContext context) : GenericRepository<User>(context), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email) =>
        DbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public Task<User?> GetByUsernameAsync(string username) =>
        DbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
}

public class ProjectRepository(TaskFlowDbContext context) : GenericRepository<Project>(context), IProjectRepository
{
    public Task<List<Project>> GetAllWithDetailsAsync() =>
        DbSet.Include(p => p.Owner).Include(p => p.Tasks)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync();

    public Task<Project?> GetByIdWithDetailsAsync(int id) =>
        DbSet.Include(p => p.Owner).Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);
}

public class TaskItemRepository(TaskFlowDbContext context) : GenericRepository<TaskItem>(context), ITaskItemRepository
{
    public Task<List<TaskItem>> GetAllWithDetailsAsync(int? projectId = null, string? status = null, int? tagId = null)
    {
        var query = DbSet
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .AsQueryable();

        if (projectId.HasValue) query = query.Where(t => t.ProjectId == projectId.Value);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(t => t.Status == status);
        if (tagId.HasValue) query = query.Where(t => t.Tags.Any(tag => tag.Id == tagId.Value));

        return query.OrderByDescending(t => t.CreatedAtUtc).ToListAsync();
    }

    public Task<TaskItem?> GetByIdWithDetailsAsync(int id) =>
        DbSet.Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Tags)
            .Include(t => t.Comments).ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(t => t.Id == id);
}
