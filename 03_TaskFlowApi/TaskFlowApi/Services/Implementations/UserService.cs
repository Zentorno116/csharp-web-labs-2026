using TaskFlowApi.DTOs;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Repositories.Interfaces;
using TaskFlowApi.Services.Interfaces;

namespace TaskFlowApi.Services.Implementations;

public class UserService(IUnitOfWork uow) : IUserService
{
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await uow.Users.GetAllAsync();
        return users.OrderBy(u => u.Id).Select(AuthService.MapUser).ToList();
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await uow.Users.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Пользователь #{id} не найден.");
        return AuthService.MapUser(user);
    }

    public async Task<UserDto> UpdateRoleAsync(int id, UpdateUserRoleDto dto)
    {
        var user = await uow.Users.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Пользователь #{id} не найден.");
        user.Role = dto.Role;
        uow.Users.Update(user);
        await uow.SaveChangesAsync();
        return AuthService.MapUser(user);
    }

    public async Task DeleteAsync(int id, int currentUserId)
    {
        if (id == currentUserId)
            throw new ApiException(400, "Нельзя удалить собственный аккаунт.");
        var user = await uow.Users.GetByIdAsync(id)
            ?? throw new ApiException(404, $"Пользователь #{id} не найден.");
        uow.Users.Remove(user);
        await uow.SaveChangesAsync();
    }
}
