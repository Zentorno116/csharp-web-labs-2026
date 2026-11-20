using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.DTOs;

public record TaskItemDto(
    int Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTime? DueDateUtc,
    DateTime CreatedAtUtc,
    int ProjectId,
    string ProjectName,
    int? AssigneeId,
    string? AssigneeUsername,
    int CommentCount,
    List<TagDto> Tags);

public record CreateTaskItemDto(
    [Required, MinLength(2), MaxLength(160)] string Title,
    [MaxLength(2000)] string Description,
    [RegularExpression("^(Todo|InProgress|Done)$")] string Status,
    [RegularExpression("^(Low|Medium|High)$")] string Priority,
    DateTime? DueDateUtc,
    [Required] int ProjectId,
    int? AssigneeId,
    List<int>? TagIds);

public record UpdateTaskItemDto(
    [Required, MinLength(2), MaxLength(160)] string Title,
    [MaxLength(2000)] string Description,
    [Required, RegularExpression("^(Todo|InProgress|Done)$")] string Status,
    [Required, RegularExpression("^(Low|Medium|High)$")] string Priority,
    DateTime? DueDateUtc,
    [Required] int ProjectId,
    int? AssigneeId,
    List<int>? TagIds);

public record CommentDto(
    int Id,
    string Content,
    DateTime CreatedAtUtc,
    int TaskItemId,
    int AuthorId,
    string AuthorUsername);

public record CreateCommentDto(
    [Required] int TaskItemId,
    [Required, MinLength(1), MaxLength(1000)] string Content);

public record UpdateCommentDto(
    [Required, MinLength(1), MaxLength(1000)] string Content);
