using Moq;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Implementations;
using Xunit;

namespace TaskFlowApi.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IProjectRepository> _projects = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _uow.SetupGet(u => u.Projects).Returns(_projects.Object);
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _service = new ProjectService(_uow.Object);
    }

    [Fact]
    public async Task CreateAsync_SavesProjectAndReturnsDto()
    {
        var owner = new User { Id = 2, Username = "timur", Email = "t@t.io", PasswordHash = "x" };
        _users.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(owner);

        var dto = await _service.CreateAsync(new CreateProjectDto("Alpha", "Desc"), 2);

        Assert.Equal("Alpha", dto.Name);
        Assert.Equal("timur", dto.OwnerUsername);
        _projects.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Throws403_WhenNotOwnerAndNotAdmin()
    {
        var proj = new Project { Id = 10, Name = "P", OwnerId = 1 };
        _projects.Setup(r => r.GetByIdWithDetailsAsync(10)).ReturnsAsync(proj);

        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _service.UpdateAsync(10, new UpdateProjectDto("New", "D", "Active"), userId: 2, isAdmin: false));
        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public async Task UpdateAsync_AllowsAdminToEditAnyProject()
    {
        var proj = new Project { Id = 10, Name = "Old", OwnerId = 1, Owner = new User { Id = 1, Username = "u", Email = "u@u", PasswordHash = "x" } };
        _projects.Setup(r => r.GetByIdWithDetailsAsync(10)).ReturnsAsync(proj);

        var res = await _service.UpdateAsync(10, new UpdateProjectDto("Updated", "NewDesc", "Completed"), userId: 99, isAdmin: true);

        Assert.Equal("Updated", res.Name);
        Assert.Equal("Completed", res.Status);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Throws404_WhenProjectMissing()
    {
        _projects.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Project?)null);

        var ex = await Assert.ThrowsAsync<ApiException>(() => _service.DeleteAsync(404, 1, true));
        Assert.Equal(404, ex.StatusCode);
    }
}
