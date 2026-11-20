using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(TaskFlowDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var admin = new User
        {
            Username = "admin",
            Email = "admin@taskflow.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "admin"
        };
        var user = new User
        {
            Username = "timur",
            Email = "timur@taskflow.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            Role = "user"
        };
        db.Users.AddRange(admin, user);
        await db.SaveChangesAsync();

        var tagBackend = new Tag { Name = "Backend", Color = "#6366f1" };
        var tagFrontend = new Tag { Name = "Frontend", Color = "#ec4899" };
        var tagUrgent = new Tag { Name = "Urgent", Color = "#ef4444" };
        var tagDb = new Tag { Name = "PostgreSQL", Color = "#14b8a6" };
        db.Tags.AddRange(tagBackend, tagFrontend, tagUrgent, tagDb);

        var proj1 = new Project
        {
            Name = "Веб-программирование (Семестр 7)",
            Description = "Комплекс проектов: LINQ, SignalR, ASP.NET Core Web API и React",
            Status = "Active",
            OwnerId = admin.Id
        };
        var proj2 = new Project
        {
            Name = "TaskFlow Platform",
            Description = "Корпоративный трекер задач с JWT авторизацией и PostgreSQL",
            Status = "Active",
            OwnerId = user.Id
        };
        db.Projects.AddRange(proj1, proj2);
        await db.SaveChangesAsync();

        var task1 = new TaskItem
        {
            Title = "Настроить PostgreSQL и миграции EF Core",
            Description = "Создать схему для 5 сущностей и применить миграции",
            Status = "Done",
            Priority = "High",
            ProjectId = proj1.Id,
            AssigneeId = admin.Id,
            DueDateUtc = DateTime.UtcNow.AddDays(2),
            Tags = [tagBackend, tagDb]
        };
        var task2 = new TaskItem
        {
            Title = "Реализовать клиентское приложение на React",
            Description = "Полный CRUD по проектам, задачам, комментариям и тегам",
            Status = "InProgress",
            Priority = "High",
            ProjectId = proj1.Id,
            AssigneeId = user.Id,
            DueDateUtc = DateTime.UtcNow.AddDays(5),
            Tags = [tagFrontend, tagUrgent]
        };
        var task3 = new TaskItem
        {
            Title = "Опубликовать сервис через ngrok",
            Description = "Подготовить публичный HTTPS туннель для демонстрации",
            Status = "Todo",
            Priority = "Medium",
            ProjectId = proj2.Id,
            AssigneeId = user.Id,
            DueDateUtc = DateTime.UtcNow.AddDays(7),
            Tags = [tagBackend]
        };
        db.TaskItems.AddRange(task1, task2, task3);
        await db.SaveChangesAsync();

        db.Comments.AddRange(
            new Comment { TaskItemId = task1.Id, AuthorId = admin.Id, Content = "Миграции успешно применены к PostgreSQL 18." },
            new Comment { TaskItemId = task2.Id, AuthorId = user.Id, Content = "Подключаю JWT Bearer авторизацию и Swagger." }
        );
        await db.SaveChangesAsync();
    }
}
