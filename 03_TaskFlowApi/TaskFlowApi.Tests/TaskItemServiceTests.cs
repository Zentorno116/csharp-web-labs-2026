using Moq;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Implementations;
using Xunit;

namespace TaskFlowApi.Tests;

public class TaskItemServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ITaskItemRepository> _tasks = new();
    private readonly Mock<IProjectRepository> _projects = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ITagRepository> _tags = new();
    private readonly TaskItemService _service;

    public TaskItemServiceTests()
    {
        _uow.SetupGet(u => u.Tasks).Returns(_tasks.Object);
        _uow.SetupGet(u => u.Projects).Returns(_projects.Object);
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _uow.SetupGet(u => u.Tags).Returns(_tags.Object);
        _service = new TaskItemService(_uow.Object);
    }

    [Fact]
    public async Task CreateAsync_AttachesProjectAssigneeAndTags()
    {
        _projects.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Project { Id = 1, Name = "Main" });
        _users.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new User { Id = 3, Username = "dev", Email = "d@d", PasswordHash = "h" });
        _tags.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([new Tag { Id = 7, Name = "API", Color = "#fff" }]);

        var dto = await _service.CreateAsync(new CreateTaskItemDto(
            "Build API", "Details", "InProgress", "High", null, ProjectId: 1, AssigneeId: 3, TagIds: [7]));

        Assert.Equal("Build API", dto.Title);
        Assert.Equal("dev", dto.AssigneeUsername);
        Assert.Single(dto.Tags);
        _tasks.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Throws404_WhenProjectNotFound()
    {
        _projects.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Project?)null);

        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _service.CreateAsync(new CreateTaskItemDto("T", "", "Todo", "Low", null, 99, null, null)));
        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingTask()
    {
        var task = new TaskItem { Id = 4, Title = "Old", ProjectId = 1 };
        _tasks.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(task);

        await _service.DeleteAsync(4);

        _tasks.Verify(r => r.Remove(task), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }
}
