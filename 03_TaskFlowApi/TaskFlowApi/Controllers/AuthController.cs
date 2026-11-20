using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.DTOs;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Controllers;

[Route("api/[controller]")]
public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto) =>
        Ok(await authService.RegisterAsync(dto));

    [HttpPost("login")]
    [HttpPost("/token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto) =>
        Ok(await authService.LoginAsync(dto));

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me() =>
        Ok(await authService.GetCurrentUserAsync(CurrentUserId));
}
