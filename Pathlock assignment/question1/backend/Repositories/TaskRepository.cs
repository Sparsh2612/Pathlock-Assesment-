using PathlockTasks.Models;

namespace PathlockTasks.Repositories;

public interface ITaskRepository
{
    IEnumerable<TaskItem> GetAll();
    TaskItem? Get(Guid id);
    TaskItem Add(TaskItem item);
    TaskItem? Update(Guid id, TaskItem item);
    bool Delete(Guid id);
}

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _items = new();

    public InMemoryTaskRepository()
    {
        _items.AddRange(new[]
        {
            new TaskItem { Id = Guid.NewGuid(), Description = "Explore Pathlock tasks app", IsCompleted = false },
            new TaskItem { Id = Guid.NewGuid(), Description = "Build backend API (.NET 8)", IsCompleted = true },
            new TaskItem { Id = Guid.NewGuid(), Description = "Polish UI with CSS", IsCompleted = false }
        });
    }

    public IEnumerable<TaskItem> GetAll() => _items.OrderBy(i => i.IsCompleted).ThenBy(i => i.Description);

    public TaskItem? Get(Guid id) => _items.FirstOrDefault(i => i.Id == id);

    public TaskItem Add(TaskItem item)
    {
        item.Id = Guid.NewGuid();
        _items.Add(item);
        return item;
    }

    public TaskItem? Update(Guid id, TaskItem item)
    {
        var existing = Get(id);
        if (existing == null) return null;
        existing.Description = item.Description;
        existing.IsCompleted = item.IsCompleted;
        return existing;
    }

    public bool Delete(Guid id)
    {
        var existing = Get(id);
        if (existing == null) return false;
        _items.Remove(existing);
        return true;
    }
}


