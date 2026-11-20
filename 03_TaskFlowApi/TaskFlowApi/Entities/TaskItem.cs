namespace TaskFlowApi.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Todo";       // Todo, InProgress, Done
    public string Priority { get; set; } = "Medium";   // Low, Medium, High
    public DateTime? DueDateUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public int? AssigneeId { get; set; }
    public User? Assignee { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
}
