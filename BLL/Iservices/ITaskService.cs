

using BLL.DTOs;
using DAL.Models.Entites;

namespace BLL.Iservices
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<TaskDto> GetTaskByIdAsync(int id);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
        Task DeleteTaskAsync(int id);
        Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(string userId);
        Task<IEnumerable<TaskDto>> GetTasksByStatusAsync(string status);
    }
}
