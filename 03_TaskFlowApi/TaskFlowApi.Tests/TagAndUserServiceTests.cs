using Moq;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Implementations;
using Xunit;

namespace TaskFlowApi.Tests;

public class TagAndUserServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ITagRepository> _tags = new();
    private readonly Mock<IUserRepository> _users = new();

    public TagAndUserServiceTests()
    {
        _uow.SetupGet(u => u.Tags).Returns(_tags.Object);
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
    }

    [Fact]
    public async Task TagService_CreateAsync_Throws409_OnDuplicateName()
    {
        _tags.Setup(r => r.GetByNameAsync("Backend")).ReturnsAsync(new Tag { Id = 1, Name = "Backend" });
        var svc = new TagService(_uow.Object);

        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            svc.CreateAsync(new CreateTagDto("Backend", "#000")));
        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task TagService_CreateAsync_SavesNewTag()
    {
        _tags.Setup(r => r.GetByNameAsync("UI")).ReturnsAsync((Tag?)null);
        var svc = new TagService(_uow.Object);

        var res = await svc.CreateAsync(new CreateTagDto("UI", "#ff00ff"));
        Assert.Equal("UI", res.Name);
        _tags.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UserService_UpdateRoleAsync_ChangesRole()
    {
        var user = new User { Id = 2, Username = "timur", Email = "t@t", PasswordHash = "h", Role = "user" };
        _users.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(user);
        var svc = new UserService(_uow.Object);

        var dto = await svc.UpdateRoleAsync(2, new UpdateUserRoleDto("admin"));
        Assert.Equal("admin", dto.Role);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UserService_DeleteAsync_Throws400_WhenDeletingSelf()
    {
        var svc = new UserService(_uow.Object);
        var ex = await Assert.ThrowsAsync<ApiException>(() => svc.DeleteAsync(5, currentUserId: 5));
        Assert.Equal(400, ex.StatusCode);
    }
}
