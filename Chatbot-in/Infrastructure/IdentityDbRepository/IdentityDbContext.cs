using Chatbot_in.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_in.Infrastructure.IdentityDbRepository
{
    public class IdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(ServerVersion.AutoDetect("Host=localhost;Database=starterauth;Username=admin;Password=admin"));
            // It would be a good idea to move the connection string to user secrets
            //options.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=mysecretpassword");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
