using Microsoft.EntityFrameworkCore;
using ApiPageDemo.Models;

namespace ApiPageDemo.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options){ }

        public DbSet<Friend> Friends { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friend>().ToTable("Friends");
        }
    }
}
