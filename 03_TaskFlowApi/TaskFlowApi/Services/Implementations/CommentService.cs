using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Services.Implementations;

public class CommentService(IUnitOfWork uow) : ICommentService
{
    public async Task<List<CommentDto>> GetByTaskIdAsync(int taskId)
    {
        var comments = await uow.Comments.GetByTaskIdWithAuthorAsync(taskId);
        return comments.Select(Map).ToList();
    }

    public async Task<CommentDto> GetByIdAsync(int id)
    {
        var comment = await uow.Comments.GetByIdWithAuthorAsync(id)
            ?? throw new ApiException(404, $"Комментарий #{id} не найден.");
        return Map(comment);
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto, int authorId)
    {
        if (await uow.Tasks.GetByIdAsync(dto.TaskItemId) == null)
            throw new ApiException(404, $"Задача #{dto.TaskItemId} не найдена.");
        var author = await uow.Users.GetByIdAsync(authorId)
            ?? throw new ApiException(404, "Автор не найден.");

        var comment = new Comment
        {
            TaskItemId = dto.TaskItemId,
            AuthorId = authorId,
            Author = author,
            Content = dto.Content.Trim()
        };
        await uow.Comments.AddAsync(comment);
        await uow.SaveChangesAsync();
        return Map(comment);
    }

    public async Task<CommentDto> UpdateAsync(int id, UpdateCommentDto dto, int userId, bool isAdmin)
    {
        var comment = await uow.Comments.GetByIdWithAuthorAsync(id)
            ?? throw new ApiException(404, $"Комментарий #{id} не найден.");
        if (!isAdmin && comment.AuthorId != userId)
            throw new ApiException(403, "Редактировать комментарий может только его автор или администратор.");

        comment.Content = dto.Content.Trim();
        uow.Comments.Update(comment);
        await uow.SaveChangesAsync();
        return Map(comment);
    }

    public async Task DeleteAsync(int id, int userId, bool isAdmin)
    {
        var comment = await uow.Comments.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Комментарий #{id} не найден.");
        if (!isAdmin && comment.AuthorId != userId)
            throw new ApiException(403, "Удалить комментарий может только его автор или администратор.");

        uow.Comments.Remove(comment);
        await uow.SaveChangesAsync();
    }

    private static CommentDto Map(Comment c) =>
        new(c.Id, c.Content, c.CreatedAtUtc, c.TaskItemId, c.AuthorId, c.Author?.Username ?? "unknown");
}
