using TaskFlowApi.Entities;

namespace TaskFlowApi.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
}

public interface IProjectRepository : IGenericRepository<Project>
{
    Task<List<Project>> GetAllWithDetailsAsync();
    Task<Project?> GetByIdWithDetailsAsync(int id);
}

public interface ITaskItemRepository : IGenericRepository<TaskItem>
{
    Task<List<TaskItem>> GetAllWithDetailsAsync(int? projectId = null, string? status = null, int? tagId = null);
    Task<TaskItem?> GetByIdWithDetailsAsync(int id);
}

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<List<Comment>> GetByTaskIdWithAuthorAsync(int taskId);
    Task<Comment?> GetByIdWithAuthorAsync(int id);
}

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name);
    Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids);
}
