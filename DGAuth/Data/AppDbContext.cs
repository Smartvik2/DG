using Microsoft.EntityFrameworkCore;
using DGAuth.Models;


namespace DGAuth.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options) { }

        //public DbSet<User> Users { get; set; }
        

        
    }
}
