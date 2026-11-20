using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Exceptions;

namespace TaskFlowApi.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new ApiException(401, "Невалидный JWT токен.");

    protected bool IsAdmin => User.IsInRole("admin");
}
