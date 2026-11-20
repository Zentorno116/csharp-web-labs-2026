using Moq;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Implementations;
using Xunit;

namespace TaskFlowApi.Tests;

public class CommentServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ICommentRepository> _comments = new();
    private readonly Mock<ITaskItemRepository> _tasks = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _uow.SetupGet(u => u.Comments).Returns(_comments.Object);
        _uow.SetupGet(u => u.Tasks).Returns(_tasks.Object);
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _service = new CommentService(_uow.Object);
    }

    [Fact]
    public async Task CreateAsync_AddsCommentForExistingTask()
    {
        _tasks.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new TaskItem { Id = 1, Title = "T", ProjectId = 1 });
        _users.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new User { Id = 2, Username = "timur", Email = "t@t", PasswordHash = "h" });

        var dto = await _service.CreateAsync(new CreateCommentDto(1, "Отлично сделано!"), 2);

        Assert.Equal("Отлично сделано!", dto.Content);
        Assert.Equal("timur", dto.AuthorUsername);
        _comments.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Throws403_WhenDifferentUserAndNotAdmin()
    {
        _comments.Setup(r => r.GetByIdAsync(8))
            .ReturnsAsync(new Comment { Id = 8, Content = "Hi", TaskItemId = 1, AuthorId = 2 });

        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _service.DeleteAsync(8, userId: 3, isAdmin: false));
        Assert.Equal(403, ex.StatusCode);
    }
}
