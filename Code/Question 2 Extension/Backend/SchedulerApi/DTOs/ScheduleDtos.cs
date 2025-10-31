namespace SchedulerApi.DTOs
{
    public class ScheduleTaskDto
    {
        public string Title { get; set; }
        public int EstimatedHours { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Dependencies { get; set; } = new();
    }

    public class ScheduleRequest
    {
        public List<ScheduleTaskDto> Tasks { get; set; } = new();
    }

    public class ScheduleResponse
    {
        public List<string> RecommendedOrder { get; set; } = new();
    }
}
