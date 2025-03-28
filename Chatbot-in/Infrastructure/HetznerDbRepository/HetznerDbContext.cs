using Chatbot_in.Domain.Identity;
using Chatbot_in.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_in.Infrastructure.HetznerDbRepository;

public class HetznerDbContext : IdentityDbContext<ApplicationUser>
{
    public HetznerDbContext(DbContextOptions<HetznerDbContext> options, IConfiguration config) : base(options) 
    {
        _configuration = config;
    }
    private readonly IConfiguration _configuration;

    public DbSet<ContextFileInfo> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //seedUser(builder); - Different - more complicated variant of seeder!
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMySql(ServerVersion.AutoDetect(_configuration["db:connectionstring"]));
        // It would be a good idea to move the connection string to user secrets
        //options.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=mysecretpassword");
    }
}
