using DAL.Identity;
using DAL.IRepository;
using DAL.Models.Data;
using DAL.Models.Entites;

namespace DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskDbContext _context;
        public ITaskRepository<User> Users { get; }
        public ITaskRepository<Tasks> Tasks { get; }

        public UnitOfWork(TaskDbContext context)
        {
            _context = context;
            Users = new TaskRepository<User>(context);
            Tasks = new TaskRepository<Tasks>(context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}