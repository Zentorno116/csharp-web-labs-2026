using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskFlowApi.Auth;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Services.Implementations;

public class AuthService(IUnitOfWork uow, IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await uow.Users.GetByEmailAsync(dto.Email) != null)
            throw new ApiException(409, "Пользователь с таким email уже существует.");
        if (await uow.Users.GetByUsernameAsync(dto.Username) != null)
            throw new ApiException(409, "Пользователь с таким именем уже существует.");

        var user = new User
        {
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "user"
        };

        await uow.Users.AddAsync(user);
        await uow.SaveChangesAsync();
        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var login = dto.Login.Trim();
        var user = await uow.Users.GetByEmailAsync(login) ?? await uow.Users.GetByUsernameAsync(login);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new ApiException(401, "Неверный логин или пароль.");

        return BuildAuthResponse(user);
    }

    public async Task<UserDto> GetCurrentUserAsync(int userId)
    {
        var user = await uow.Users.GetByIdAsync(userId)
            ?? throw new ApiException(404, "Пользователь не найден.");
        return MapUser(user);
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_jwt.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: new SigningCredentials(_jwt.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return new AuthResponseDto(new JwtSecurityTokenHandler().WriteToken(token), expires, MapUser(user));
    }

    internal static UserDto MapUser(User u) => new(u.Id, u.Username, u.Email, u.Role, u.CreatedAtUtc);
}
