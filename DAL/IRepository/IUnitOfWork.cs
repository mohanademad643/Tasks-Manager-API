using DAL.Identity;
using DAL.Models.Entites;

namespace DAL.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Tasks> Tasks { get; }
        Task<int> SaveChangesAsync();
    }
}