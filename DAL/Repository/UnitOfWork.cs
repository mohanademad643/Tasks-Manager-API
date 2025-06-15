using DAL.Identity;
using DAL.IRepository;
using DAL.Models.Data;
using DAL.Models.Entites;

namespace DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskDbContext _context;
        public IRepository<User> Users { get; }
        public IRepository<Tasks> Tasks { get; }

        public UnitOfWork(TaskDbContext context)
        {
            _context = context;
            Users = new Repository<User>(context);
            Tasks = new Repository<Tasks>(context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}