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
    }
}
