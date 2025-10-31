namespace ProjectManager.DTOs
{
    public class ProjectCreateRequest
    {
        public string Title { get; set; }
    }

    public class ProjectUpdateRequest
    {
        public string Title { get; set; }
    }

    public class ProjectResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
