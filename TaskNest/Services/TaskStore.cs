using TaskNest.Models;

namespace TaskNest.Services;

// Thread-safe in-memory store. Data intentionally resets when the app restarts.
public sealed class TaskStore
{
    private readonly object _sync = new();
    private readonly List<TaskItem> _items = new()
    {
        new() { Id = 1, Title = "Plan the sprint", Description = "Review priorities and organize the next sprint.", Category = "Work", Priority = "High", DueDate = DateTime.Today.AddDays(2) },
        new() { Id = 2, Title = "Read a chapter", Description = "Make progress on a personal learning goal.", Category = "Personal", Priority = "Medium", DueDate = DateTime.Today.AddDays(5) },
        new() { Id = 3, Title = "Update project notes", Description = "Document the latest decisions.", Category = "Study", Priority = "Low", IsCompleted = true }
    };
    private int _nextId = 4;

    private static TaskItem Copy(TaskItem x) => new()
    {
        Id = x.Id, Title = x.Title, Description = x.Description, Category = x.Category,
        Priority = x.Priority, DueDate = x.DueDate, IsCompleted = x.IsCompleted,
        CreatedAtUtc = x.CreatedAtUtc
    };

    public List<TaskItem> All() { lock (_sync) return _items.Select(Copy).ToList(); }
    public TaskItem? Get(int id) { lock (_sync) { var x = _items.FirstOrDefault(t => t.Id == id); return x is null ? null : Copy(x); } }
    public void Add(TaskItem task)
    {
        lock (_sync)
        {
            var x = Copy(task);
            x.Id = _nextId++;
            x.CreatedAtUtc = DateTime.UtcNow;
            _items.Add(x);
        }
    }
    public bool Update(TaskItem task)
    {
        lock (_sync)
        {
            var x = _items.FirstOrDefault(t => t.Id == task.Id);
            if (x is null) return false;
            x.Title = task.Title; x.Description = task.Description; x.Category = task.Category;
            x.Priority = task.Priority; x.DueDate = task.DueDate;
            return true;
        }
    }
    public bool Toggle(int id)
    {
        lock (_sync)
        {
            var x = _items.FirstOrDefault(t => t.Id == id);
            if (x is null) return false;
            x.IsCompleted = !x.IsCompleted;
            return true;
        }
    }
    public bool Delete(int id)
    {
        lock (_sync)
        {
            var x = _items.FirstOrDefault(t => t.Id == id);
            return x is not null && _items.Remove(x);
        }
    }
}
