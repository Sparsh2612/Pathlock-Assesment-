using PathlockProjects.Models;

namespace PathlockProjects.Services;

public interface IDataStore
{
    User AddUser(User user);
    User? FindUser(string username);
    IEnumerable<Project> GetProjects(Guid ownerId);
    Project AddProject(Project project);
    Project? GetProject(Guid ownerId, Guid id);
    bool DeleteProject(Guid ownerId, Guid id);

    TaskItem AddTask(TaskItem task);
    TaskItem? GetTask(Guid ownerId, Guid id);
    TaskItem? UpdateTask(TaskItem updated);
    bool DeleteTask(Guid ownerId, Guid id);
}

public class InMemoryStore : IDataStore
{
    private readonly List<User> _users = new();
    private readonly List<Project> _projects = new();
    private readonly List<TaskItem> _tasks = new();

    public User AddUser(User user)
    {
        _users.Add(user);
        return user;
    }

    public User? FindUser(string username) => _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Project> GetProjects(Guid ownerId) => _projects.Where(p => p.OwnerId == ownerId).OrderByDescending(p => p.CreatedAt);

    public Project AddProject(Project project)
    {
        _projects.Add(project);
        return project;
    }

    public Project? GetProject(Guid ownerId, Guid id) => _projects.FirstOrDefault(p => p.OwnerId == ownerId && p.Id == id);

    public bool DeleteProject(Guid ownerId, Guid id)
    {
        var p = GetProject(ownerId, id);
        if (p == null) return false;
        _tasks.RemoveAll(t => t.OwnerId == ownerId && t.ProjectId == id);
        _projects.Remove(p);
        return true;
    }

    public TaskItem AddTask(TaskItem task)
    {
        _tasks.Add(task);
        return task;
    }

    public TaskItem? GetTask(Guid ownerId, Guid id) => _tasks.FirstOrDefault(t => t.OwnerId == ownerId && t.Id == id);

    public TaskItem? UpdateTask(TaskItem updated)
    {
        var t = GetTask(updated.OwnerId, updated.Id);
        if (t == null) return null;
        t.Title = updated.Title;
        t.DueDate = updated.DueDate;
        t.IsCompleted = updated.IsCompleted;
        return t;
    }

    public bool DeleteTask(Guid ownerId, Guid id)
    {
        var t = GetTask(ownerId, id);
        if (t == null) return false;
        _tasks.Remove(t);
        return true;
    }
}


