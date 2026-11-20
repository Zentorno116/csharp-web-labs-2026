using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.DTOs;

public record ProjectDto(
    int Id,
    string Name,
    string Description,
    string Status,
    DateTime CreatedAtUtc,
    int OwnerId,
    string OwnerUsername,
    int TaskCount);

public record CreateProjectDto(
    [Required, MinLength(2), MaxLength(120)] string Name,
    [MaxLength(1000)] string Description = "",
    [RegularExpression("^(Active|Completed|Archived)$")] string Status = "Active");

public record UpdateProjectDto(
    [Required, MinLength(2), MaxLength(120)] string Name,
    [MaxLength(1000)] string Description,
    [Required, RegularExpression("^(Active|Completed|Archived)$")] string Status);

public record TagDto(int Id, string Name, string Color);

public record CreateTagDto(
    [Required, MinLength(1), MaxLength(40)] string Name,
    [Required, MaxLength(20)] string Color = "#6366f1");

public record UpdateTagDto(
    [Required, MinLength(1), MaxLength(40)] string Name,
    [Required, MaxLength(20)] string Color);
