using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.DTOs;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class TagsController(ITagService tagService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetAll() =>
        Ok(await tagService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TagDto>> GetById(int id) =>
        Ok(await tagService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagDto dto)
    {
        var created = await tagService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TagDto>> Update(int id, [FromBody] UpdateTagDto dto) =>
        Ok(await tagService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await tagService.DeleteAsync(id);
        return NoContent();
    }
}
