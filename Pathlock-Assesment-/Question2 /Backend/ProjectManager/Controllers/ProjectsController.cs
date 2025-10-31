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
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProjectsController(AppDbContext db) { _db = db; }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"));

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjects()
        {
            var userId = GetUserId();
            var projects = await _db.Projects.Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProjectResponse { Id = p.Id, Title = p.Title, CreatedAt = p.CreatedAt })
                .ToListAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetById(int id)
        {
            var userId = GetUserId();
            var project = await _db.Projects.Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectResponse>> Create(ProjectCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest("Title is required");
            var project = new Project { Title = request.Title, UserId = GetUserId() };
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            var response = new ProjectResponse { Id = project.Id, Title = project.Title, CreatedAt = project.CreatedAt };
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProjectUpdateRequest request)
        {
            var userId = GetUserId();
            var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return NotFound();
            if (string.IsNullOrWhiteSpace(request.Title)) return BadRequest("Title is required");
            project.Title = request.Title;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return NotFound();
            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
