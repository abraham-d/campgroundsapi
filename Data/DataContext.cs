using campgrounds_api.Models;
using Microsoft.EntityFrameworkCore;

namespace campgrounds_api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Campground> Campgrounds { get; set; }
        public DbSet<Photo> Photos { get; set; }

    }
}