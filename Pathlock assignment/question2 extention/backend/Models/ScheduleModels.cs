using System.ComponentModel.DataAnnotations;

namespace SmartScheduler.Models;

public class ScheduleTaskInput
{
    [Required, MinLength(1), MaxLength(200)] public string Title { get; set; } = string.Empty;
    [Range(1, 1000)] public int EstimatedHours { get; set; }
    public DateOnly? DueDate { get; set; }
    public List<string> Dependencies { get; set; } = new();
}

public class ScheduleRequest
{
    [Required] public List<ScheduleTaskInput> Tasks { get; set; } = new();
}

public class ScheduleResponse
{
    public List<string> RecommendedOrder { get; set; } = new();
}


