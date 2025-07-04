
using DAL.Models.Entites;

namespace DAL.IRepository
{
    public interface ITaskRepository<T> where T : class
    {
        Task<IEnumerable<Tasks>> GetAllByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllUserAsync();
        Task<IEnumerable<Tasks>> GetAllTasksByTitle(string title);
        Task<T> UpdateTaskStatusAsync(int taskId, string userId, string newStatus);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(int id);
    }
}
