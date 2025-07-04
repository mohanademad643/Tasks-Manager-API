

using BLL.DTOs;
using DAL.Models.Entites;

namespace BLL.Iservices
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<TaskDto>> SearchTasksByTitleAsync(string title);
        Task<bool> UpdateUserStatusAsync(string userId, string status);
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<TaskDto> GetTaskByIdAsync(int id);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
        Task DeleteTaskAsync(int id);
        Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(string userId);
        Task<IEnumerable<TaskDto>> GetTasksByStatusAsync(string status);
        Task<TaskDto> UpdateTaskStatusAsync(int taskId, string userId, string newStatus);
    }
}
