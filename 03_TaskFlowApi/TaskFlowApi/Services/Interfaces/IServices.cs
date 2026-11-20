using TaskFlowApi.DTOs;

namespace TaskFlowApi.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto> GetCurrentUserAsync(int userId);
}

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
    Task<UserDto> UpdateRoleAsync(int id, UpdateUserRoleDto dto);
    Task DeleteAsync(int id, int currentUserId);
}

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto, int ownerId);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto, int userId, bool isAdmin);
    Task DeleteAsync(int id, int userId, bool isAdmin);
}

public interface ITaskItemService
{
    Task<List<TaskItemDto>> GetAllAsync(int? projectId = null, string? status = null, int? tagId = null);
    Task<TaskItemDto> GetByIdAsync(int id);
    Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto);
    Task<TaskItemDto> UpdateAsync(int id, UpdateTaskItemDto dto);
    Task DeleteAsync(int id);
}

public interface ICommentService
{
    Task<List<CommentDto>> GetByTaskIdAsync(int taskId);
    Task<CommentDto> GetByIdAsync(int id);
    Task<CommentDto> CreateAsync(CreateCommentDto dto, int authorId);
    Task<CommentDto> UpdateAsync(int id, UpdateCommentDto dto, int userId, bool isAdmin);
    Task DeleteAsync(int id, int userId, bool isAdmin);
}

public interface ITagService
{
    Task<List<TagDto>> GetAllAsync();
    Task<TagDto> GetByIdAsync(int id);
    Task<TagDto> CreateAsync(CreateTagDto dto);
    Task<TagDto> UpdateAsync(int id, UpdateTagDto dto);
    Task DeleteAsync(int id);
}
