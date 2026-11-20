namespace TaskFlowApi.Entities;

public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Color { get; set; } = "#6366f1";

    public ICollection<TaskItem> Tasks { get; set; } = [];
}
