
using DAL.Models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DAL.Models.Data.Config
{
    public class TaskConfiguration : IEntityTypeConfiguration<Tasks>
    {
        public void Configure(EntityTypeBuilder<Tasks> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).IsRequired()
             .UseIdentityColumn(1, 1);

            builder.Property(t => t.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(450); 

            builder.Property(t => t.Deadline)
                .IsRequired(false);

            builder.Property(t => t.Image)
                .HasMaxLength(500); 

            builder.Property(t => t.Description)
                .HasMaxLength(1000); 

            builder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(t => t.User)
                  .WithMany(u => u.Tasks) 
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);


            builder.ToTable("Tasks");
        }
    }
}
