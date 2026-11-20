using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Services.Implementations;

public class TagService(IUnitOfWork uow) : ITagService
{
    public async Task<List<TagDto>> GetAllAsync()
    {
        var tags = await uow.Tags.GetAllAsync();
        return tags.OrderBy(t => t.Name).Select(t => new TagDto(t.Id, t.Name, t.Color)).ToList();
    }

    public async Task<TagDto> GetByIdAsync(int id)
    {
        var tag = await uow.Tags.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Тег #{id} не найден.");
        return new TagDto(tag.Id, tag.Name, tag.Color);
    }

    public async Task<TagDto> CreateAsync(CreateTagDto dto)
    {
        var name = dto.Name.Trim();
        if (await uow.Tags.GetByNameAsync(name) != null)
            throw new ApiException(409, $"Тег «{name}» уже существует.");

        var tag = new Tag { Name = name, Color = dto.Color.Trim() };
        await uow.Tags.AddAsync(tag);
        await uow.SaveChangesAsync();
        return new TagDto(tag.Id, tag.Name, tag.Color);
    }

    public async Task<TagDto> UpdateAsync(int id, UpdateTagDto dto)
    {
        var tag = await uow.Tags.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Тег #{id} не найден.");

        var existing = await uow.Tags.GetByNameAsync(dto.Name.Trim());
        if (existing != null && existing.Id != id)
            throw new ApiException(409, $"Тег «{dto.Name.Trim()}» уже существует.");

        tag.Name = dto.Name.Trim();
        tag.Color = dto.Color.Trim();
        uow.Tags.Update(tag);
        await uow.SaveChangesAsync();
        return new TagDto(tag.Id, tag.Name, tag.Color);
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await uow.Tags.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Тег #{id} не найден.");
        uow.Tags.Remove(tag);
        await uow.SaveChangesAsync();
    }
}
