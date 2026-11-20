using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.DTOs;

public record RegisterDto(
    [Required, MinLength(2), MaxLength(50)] string Username,
    [Required, EmailAddress, MaxLength(120)] string Email,
    [Required, MinLength(6), MaxLength(100)] string Password);

public record LoginDto(
    [Required] string Login, // email or username
    [Required] string Password);

public record AuthResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserDto User);

public record UserDto(
    int Id,
    string Username,
    string Email,
    string Role,
    DateTime CreatedAtUtc);

public record UpdateUserRoleDto(
    [Required, RegularExpression("^(admin|user)$")] string Role);
