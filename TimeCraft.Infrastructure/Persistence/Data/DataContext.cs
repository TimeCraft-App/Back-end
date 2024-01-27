using Microsoft.EntityFrameworkCore;
using TimeCraft.Domain.Entities;

namespace TimeCraft.Infrastructure.Persistence.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<QueuedEmail> QueuedEmails { get; set; }
        public DbSet<TimeoffBalance> TimeoffBalances { get; set; }
        public DbSet<TimeoffRequest> TimeoffRequests { get; set; }
        public DbSet<TimeWorked> TimeWorked { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TimeWorked>()
                .HasOne(tw => tw.Project)
                .WithMany(p => p.TimeWorked)
                .HasForeignKey(tw => tw.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
