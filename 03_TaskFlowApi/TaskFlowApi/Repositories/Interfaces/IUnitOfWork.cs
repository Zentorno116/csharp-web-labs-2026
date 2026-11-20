namespace TaskFlowApi.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProjectRepository Projects { get; }
    ITaskItemRepository Tasks { get; }
    ICommentRepository Comments { get; }
    ITagRepository Tags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
