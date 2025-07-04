using DAL.Identity;
using DAL.Models.Data.Config;
using DAL.Models.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace DAL.Models.Data
{
    public class TaskDbContext: IdentityDbContext<User>
    {  
        public DbSet<Tasks> Task { get; set; }
        public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TaskConfiguration());
        }

    }
}
