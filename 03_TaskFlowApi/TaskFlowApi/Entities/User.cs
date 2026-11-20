namespace TaskFlowApi.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = "user";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Project> OwnedProjects { get; set; } = [];
    public ICollection<TaskItem> AssignedTasks { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}
