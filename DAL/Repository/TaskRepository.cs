using DAL.Identity;
using DAL.IRepository;
using DAL.Models.Data;
using DAL.Models.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL.Repository
{
    public class TaskRepository<T> : ITaskRepository<T> where T : class
    {
        protected readonly TaskDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public TaskRepository(TaskDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> UpdateTaskStatusAsync(int taskId, string userId, string newStatus)
        {
            var task = await _context.Set<T>()
                .Include(t => (t as Tasks).User)
                .FirstOrDefaultAsync(t => (t as Tasks).Id == taskId && (t as Tasks).UserId == userId && (t as Tasks).Status !=null );

            if (task == null)
            {
                return null;
            }

          (task as Tasks).Status = newStatus;
            await _context.SaveChangesAsync();
            return task;
        }
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.Include(t => (t as Tasks).User).ToListAsync();
        public async Task<IEnumerable<Tasks>> GetAllTasksByTitle(string title) {
            return await _context.Task.Include(t => t.User)
              .Where(t => t.Title.Contains(title))
              .ToListAsync();
        }
        public async Task<IEnumerable<Tasks>> GetAllByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date cannot be later than end date.");
            }

            return await _context.Set<Tasks>()
                .Include(t => t.User)
                .Where(t => t.Deadline >= startDate && t.Deadline <= endDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllUserAsync() => await _dbSet.ToListAsync();
        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);

        public async Task<bool> ExistsAsync(int id) => await _dbSet.FindAsync(id) != null;
    }
}