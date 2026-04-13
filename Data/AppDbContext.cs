using Microsoft.EntityFrameworkCore;      // Без этого нет встроенного DbContext
using Web.Models.Database;

namespace Web.Data
{
    public class AppDbContext : DbContext    //AppDbContext наследуется от DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();
        }
    }
}