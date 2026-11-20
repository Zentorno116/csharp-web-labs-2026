namespace TaskFlowApi.Entities;

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int OwnerId { get; set; }
    public User? Owner { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = [];
}
