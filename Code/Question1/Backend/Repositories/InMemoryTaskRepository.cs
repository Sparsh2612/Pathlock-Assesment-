using System;
using System.Collections.Generic;
using System.Linq;
using CodingAssignment.Models;

namespace CodingAssignment.Repositories
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = new List<TaskItem>();

        public IEnumerable<TaskItem> GetAll() => _tasks;

        public TaskItem GetById(Guid id) => _tasks.FirstOrDefault(t => t.Id == id);

        public void Add(TaskItem task)
        {
            _tasks.Add(task);
        }

        public void Update(TaskItem task)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
            {
                existing.Description = task.Description;
                existing.IsCompleted = task.IsCompleted;
            }
        }

        public void Delete(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
                _tasks.Remove(task);
        }
    }
}
