using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Entities;
using TaskFlowApi.Repositories.Interfaces;

namespace TaskFlowApi.Repositories.Implementations;

public class CommentRepository(TaskFlowDbContext context) : GenericRepository<Comment>(context), ICommentRepository
{
    public Task<List<Comment>> GetByTaskIdWithAuthorAsync(int taskId) =>
        DbSet.Include(c => c.Author)
            .Where(c => c.TaskItemId == taskId)
            .OrderBy(c => c.CreatedAtUtc)
            .ToListAsync();

    public Task<Comment?> GetByIdWithAuthorAsync(int id) =>
        DbSet.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == id);
}

public class TagRepository(TaskFlowDbContext context) : GenericRepository<Tag>(context), ITagRepository
{
    public Task<Tag?> GetByNameAsync(string name) =>
        DbSet.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

    public Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.Distinct().ToList();
        return DbSet.Where(t => idList.Contains(t.Id)).ToListAsync();
    }
}

public class UnitOfWork(
    TaskFlowDbContext context,
    IUserRepository users,
    IProjectRepository projects,
    ITaskItemRepository tasks,
    ICommentRepository comments,
    ITagRepository tags) : IUnitOfWork
{
    public IUserRepository Users => users;
    public IProjectRepository Projects => projects;
    public ITaskItemRepository Tasks => tasks;
    public ICommentRepository Comments => comments;
    public ITagRepository Tags => tags;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public void Dispose() => context.Dispose();
}
