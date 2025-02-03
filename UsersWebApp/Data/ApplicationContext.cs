using Microsoft.EntityFrameworkCore;
using UsersWebApp.Models;

namespace UsersWebApp.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
