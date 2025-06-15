using DAL.IRepository;
using DAL.Models.Data;
using DAL.Models.Entites;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TaskDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(TaskDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.Include(t => (t as Tasks).User).ToListAsync();
        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);

        public async Task<bool> ExistsAsync(int id) => await _dbSet.FindAsync(id) != null;
    }
}