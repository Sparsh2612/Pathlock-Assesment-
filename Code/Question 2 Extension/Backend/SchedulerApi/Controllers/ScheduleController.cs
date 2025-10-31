using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DTOs;
using SchedulerApi.Services;

namespace SchedulerApi.Controllers
{
    [ApiController]
    [Route("api/v1/projects/{projectId}/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ISchedulerService _service;
        public ScheduleController(ISchedulerService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ScheduleResponse> Schedule(int projectId, [FromBody] ScheduleRequest request)
        {
            if (request == null || request.Tasks == null || request.Tasks.Count == 0)
                return BadRequest("Tasks are required");
            var result = _service.Recommend(request);
            return Ok(result);
        }
    }
}
