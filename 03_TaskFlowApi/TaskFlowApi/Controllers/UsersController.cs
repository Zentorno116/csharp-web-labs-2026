using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.DTOs;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll() =>
        Ok(await userService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id) =>
        Ok(await userService.GetByIdAsync(id));

    [HttpPut("{id:int}/role")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> UpdateRole(int id, [FromBody] UpdateUserRoleDto dto) =>
        Ok(await userService.UpdateRoleAsync(id, dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await userService.DeleteAsync(id, CurrentUserId);
        return NoContent();
    }
}
