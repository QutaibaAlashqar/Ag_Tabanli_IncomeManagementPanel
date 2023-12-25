
using Microsoft.EntityFrameworkCore;
using WebApplication1.Pages;

namespace WebApplication1.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Box> Boxes { get; set; }

    }
}
