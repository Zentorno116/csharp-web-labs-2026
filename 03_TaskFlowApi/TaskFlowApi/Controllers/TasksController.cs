using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.DTOs;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class TasksController(ITaskItemService taskService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<TaskItemDto>>> GetAll(
        [FromQuery] int? projectId,
        [FromQuery] string? status,
        [FromQuery] int? tagId) =>
        Ok(await taskService.GetAllAsync(projectId, status, tagId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItemDto>> GetById(int id) =>
        Ok(await taskService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<TaskItemDto>> Create([FromBody] CreateTaskItemDto dto)
    {
        var created = await taskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskItemDto>> Update(int id, [FromBody] UpdateTaskItemDto dto) =>
        Ok(await taskService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await taskService.DeleteAsync(id);
        return NoContent();
    }
}
