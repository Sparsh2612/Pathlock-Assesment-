using System.ComponentModel.DataAnnotations;

namespace PathlockProjects.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MinLength(3), MaxLength(50)] public string Username { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
}

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MinLength(3), MaxLength(100)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required] public Guid OwnerId { get; set; }
}

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MinLength(1), MaxLength(200)] public string Title { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    [Required] public Guid ProjectId { get; set; }
    [Required] public Guid OwnerId { get; set; }
}

public record RegisterRequest([Required, MinLength(3), MaxLength(50)] string Username,
                               [Required, MinLength(4), MaxLength(100)] string Password);

public record LoginRequest([Required] string Username, [Required] string Password);

public record ProjectCreateRequest([Required, MinLength(3), MaxLength(100)] string Title,
                                   [MaxLength(500)] string? Description);

public record TaskCreateRequest([Required, MinLength(1), MaxLength(200)] string Title,
                                DateTime? DueDate);

public record TaskUpdateRequest([Required] bool IsCompleted, [Required, MinLength(1), MaxLength(200)] string Title, DateTime? DueDate);


