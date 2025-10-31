using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.DTOs;
using ProjectManager.Models;
using System.Security.Claims;
using System.Linq;

namespace ProjectManager.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/projects/{projectId}/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TasksController(AppDbContext db) { _db = db; }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"));

        private async Task<Project?> GetUserProject(int projectId)
        {
            var userId = GetUserId();
            return await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponse>> Create(int projectId, TaskCreateRequest request)
        {
            var project = await GetUserProject(projectId);
            if (project == null) return NotFound();
            if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest("Title is required");

            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                ProjectId = projectId
            };
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            var response = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                ProjectId = task.ProjectId
            };
            return CreatedAtAction(nameof(Create), new { projectId, id = task.Id }, response);
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> Update(int projectId, int taskId, TaskUpdateRequest request)
        {
            var project = await GetUserProject(projectId);
            if (project == null) return NotFound();
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
            if (task == null) return NotFound();

            if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest("Title is required");
            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.IsCompleted = request.IsCompleted;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> Delete(int projectId, int taskId)
        {
            var project = await GetUserProject(projectId);
            if (project == null) return NotFound();
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
            if (task == null) return NotFound();
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
