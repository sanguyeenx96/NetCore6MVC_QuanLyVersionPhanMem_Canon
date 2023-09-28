using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Quanlyversion.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        // Override OnModelCreating to configure entity relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // DbSet for your entities
        public DbSet<Model> Models { get; set; }

        public DbSet<Software> Softwares { get; set; }

        public DbSet<Password> Passwords { get; set; }
        public DbSet<Lichsu> Lichsus { get; set; }

    }
}