namespace ProjectManager.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; }
        public List<TaskItem> Tasks { get; set; } = new();
    }
}