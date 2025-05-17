using Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;
using System.Reflection.Metadata;
using ToDoListServer.Hubs;
using ToDoListServer.Interfaces;

namespace ToDoListServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IHubContext<TaskHub> _hub;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, IHubContext<TaskHub> hub, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _hub = hub;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _taskService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TaskDto dto)
        {
            var username = HttpContext.User?.Identity?.Name ?? "guest";

            try
            {
                var result = await _taskService.AddAsync(dto, username);
                await _hub.Clients.All.SendAsync("TaskAdded", result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaskDto dto)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "guest";
                var result = await _taskService.UpdateAsync(dto, username);
                await _hub.Clients.All.SendAsync("TaskUpdated", result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var username = HttpContext.User?.Identity?.Name ?? "guest";
            try
            {
                await _taskService.DeleteAsync(id, username);
                await _hub.Clients.All.SendAsync("TaskDeleted", id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("lock/{id}")]
        public async Task<IActionResult> Lock(Guid id)
        {
            var username = HttpContext.User?.Identity?.Name ?? "guest";

            try
            {
                var success = await _taskService.LockAsync(id, username);

                if (success)
                {
                    await _hub.Clients.All.SendAsync("TaskLocked", id, username);
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Failed to lock task");
        }

        [HttpPost("unlock/{id}")]
        public async Task<IActionResult> Unlock(Guid id)
        {
            var username = HttpContext.User?.Identity?.Name ?? "guest";
            try
            {
                var success = await _taskService.UnlockAsync(id, username);

                if (success)
                {
                    await _hub.Clients.All.SendAsync("TaskUnlocked", id);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Failed to unlock task");
        }
    }

}
