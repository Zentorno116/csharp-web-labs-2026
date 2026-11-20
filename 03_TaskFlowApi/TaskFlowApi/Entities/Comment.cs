namespace TaskFlowApi.Entities;

public class Comment
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public int AuthorId { get; set; }
    public User? Author { get; set; }
}
