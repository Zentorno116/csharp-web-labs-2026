using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.DTOs;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ProjectsController(IProjectService projectService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll() =>
        Ok(await projectService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id) =>
        Ok(await projectService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto)
    {
        var created = await projectService.CreateAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] UpdateProjectDto dto) =>
        Ok(await projectService.UpdateAsync(id, dto, CurrentUserId, IsAdmin));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await projectService.DeleteAsync(id, CurrentUserId, IsAdmin);
        return NoContent();
    }
}
