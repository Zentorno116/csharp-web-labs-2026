using Microsoft.Extensions.Options;
using Moq;
using TaskFlowApi.Auth;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Implementations;
using Xunit;

namespace TaskFlowApi.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _service = new AuthService(_uow.Object, Options.Create(new JwtSettings()));
    }

    [Fact]
    public async Task RegisterAsync_CreatesUserAndReturnsJwt()
    {
        _users.Setup(r => r.GetByEmailAsync("new@test.io")).ReturnsAsync((User?)null);
        _users.Setup(r => r.GetByUsernameAsync("newuser")).ReturnsAsync((User?)null);

        var res = await _service.RegisterAsync(new RegisterDto("newuser", "new@test.io", "Secret123!"));

        Assert.False(string.IsNullOrWhiteSpace(res.AccessToken));
        Assert.Equal("newuser", res.User.Username);
        Assert.Equal("user", res.User.Role);
        _users.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_Throws409_WhenEmailExists()
    {
        _users.Setup(r => r.GetByEmailAsync("dup@test.io"))
            .ReturnsAsync(new User { Id = 1, Username = "u", Email = "dup@test.io", PasswordHash = "h" });

        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _service.RegisterAsync(new RegisterDto("other", "dup@test.io", "Secret123!")));
        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_ReturnsJwt_WhenPasswordValid()
    {
        var user = new User
        {
            Id = 5, Username = "admin", Email = "admin@test.io",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!"), Role = "admin"
        };
        _users.Setup(r => r.GetByEmailAsync("admin@test.io")).ReturnsAsync(user);

        var res = await _service.LoginAsync(new LoginDto("admin@test.io", "Pass123!"));

        Assert.Equal("admin", res.User.Role);
        Assert.Contains('.', res.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_Throws401_WhenPasswordWrong()
    {
        var user = new User
        {
            Id = 5, Username = "admin", Email = "admin@test.io",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!")
        };
        _users.Setup(r => r.GetByEmailAsync("admin@test.io")).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<ApiException>(() =>
            _service.LoginAsync(new LoginDto("admin@test.io", "WrongPass")));
        Assert.Equal(401, ex.StatusCode);
    }
}
