using AuthApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options) {}
        public DbSet<User> Users { get; set; }


    }
}