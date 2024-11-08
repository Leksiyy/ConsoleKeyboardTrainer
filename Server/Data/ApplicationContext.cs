using KeyboardTrainer.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Statistics> Statistics { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
            base.OnConfiguring(optionsBuilder);
        }

    }

}