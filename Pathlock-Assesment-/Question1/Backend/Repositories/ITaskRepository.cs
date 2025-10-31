using System;
using System.Collections.Generic;
using CodingAssignment.Models;

namespace CodingAssignment.Repositories
{
    public interface ITaskRepository
    {
        IEnumerable<TaskItem> GetAll();
        TaskItem GetById(Guid id);
        void Add(TaskItem task);
        void Update(TaskItem task);
        void Delete(Guid id);
    }
}
