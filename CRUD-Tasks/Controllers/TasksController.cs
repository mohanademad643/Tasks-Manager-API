using BLL.DTOs;
using BLL.Iservices;
using DAL.Models.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _taskService.GetAllUsersAsync();

        return Ok(users);
    }
    [HttpGet("GetAllTasks")]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        
        return Ok(tasks);
    }

    [HttpGet("GetTaskById/{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(task);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("CreateTask")]
    //[Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateTask([FromForm] CreateTaskDto createTaskDto)
    {
        var task = await _taskService.CreateTaskAsync(createTaskDto);
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    [HttpPut("{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus(string userId, [FromBody] string statusDto)
    {
        if (string.IsNullOrEmpty(statusDto))
        {
            return BadRequest("Status cannot be empty.");
        }

        var result = await _taskService.UpdateUserStatusAsync(userId, statusDto);
        if (!result)
        {
            return NotFound("User not found.");
        }

        return Ok("User status updated successfully.");
    }
    [HttpPut("UpdateTask/{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromForm] UpdateTaskDto updateTaskDto)
    {
        try
        {
            await _taskService.UpdateTaskAsync(id, updateTaskDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpPut("UpdateUserTaskStatus")]
    public async Task<IActionResult> UpdateUsetTaskStatus(int TaskId , string userId , string NewStatus)
    {
        var updatedTask = await _taskService.UpdateTaskStatusAsync(TaskId, userId, NewStatus);
        if (updatedTask == null)
        {
            return BadRequest("Task not found, not owned by user, or not in 'InProgress' status.");
        }

        return Ok("Update User Task Status successfully");
    }

    [HttpDelete("DeleteTask/{id}")]
    //[Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("GetTasksByUser/{userId}")]
    public async Task<IActionResult> GetTasksByUser(string userId)
    {
        var tasks = await _taskService.GetTasksByUserIdAsync(userId);
        return Ok(tasks);
    }
    [HttpGet("GetUserTasksByTitle")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> SearchTasksByTitle([FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Title cannot be empty.");
        }

        var tasks = await _taskService.SearchTasksByTitleAsync(title);
        return Ok(tasks);
    }
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest("Start date cannot be later than end date.");
        }

        var tasks = await _taskService.GetTasksByDateRangeAsync(startDate, endDate);
        return Ok(tasks);
    }
    [HttpGet("GetTasksByStatus/{status}")]
    public async Task<IActionResult> GetTasksByStatus(string status)
    {
        var tasks = await _taskService.GetTasksByStatusAsync(status);
        return Ok(tasks);
    }
}