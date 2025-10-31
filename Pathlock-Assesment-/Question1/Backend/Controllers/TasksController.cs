using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using CodingAssignment.Models;
using CodingAssignment.Repositories;

namespace CodingAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _repository;

        public TasksController(ITaskRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetAll()
            => Ok(_repository.GetAll());

        [HttpPost]
        public ActionResult<TaskItem> Add(TaskItem task)
        {
            task.Id = Guid.NewGuid();
            _repository.Add(task);
            return CreatedAtAction(nameof(GetAll), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, TaskItem task)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();
            task.Id = id; // Ensure Id stays the same
            _repository.Update(task);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();
            _repository.Delete(id);
            return NoContent();
        }
    }
}
