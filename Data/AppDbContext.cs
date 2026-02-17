
using Microsoft.EntityFrameworkCore;
using  TaskManagement.Models;


namespace TaskManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItems> Tasks => Set<TaskItems>();
        public DbSet<user> Users => Set<user>();
    }
}
