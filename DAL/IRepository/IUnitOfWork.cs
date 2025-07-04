using DAL.Identity;
using DAL.Models.Entites;

namespace DAL.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository<User> Users { get; }
        ITaskRepository<Tasks> Tasks { get; }
        Task<int> SaveChangesAsync();
    }
}