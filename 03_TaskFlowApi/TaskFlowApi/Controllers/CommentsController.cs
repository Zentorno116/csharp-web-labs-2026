using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.DTOs;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class CommentsController(ICommentService commentService) : BaseApiController
{
    [HttpGet("task/{taskId:int}")]
    public async Task<ActionResult<List<CommentDto>>> GetByTask(int taskId) =>
        Ok(await commentService.GetByTaskIdAsync(taskId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetById(int id) =>
        Ok(await commentService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto dto)
    {
        var created = await commentService.CreateAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDto>> Update(int id, [FromBody] UpdateCommentDto dto) =>
        Ok(await commentService.UpdateAsync(id, dto, CurrentUserId, IsAdmin));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await commentService.DeleteAsync(id, CurrentUserId, IsAdmin);
        return NoContent();
    }
}
