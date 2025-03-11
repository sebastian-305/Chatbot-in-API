using Chatbot_in.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_in.Infrastructure.HetznerDbRepository;

public class HetznerDbContext : DbContext
{
    public HetznerDbContext(DbContextOptions<HetznerDbContext> options) : base(options) { }

    public DbSet<ContextFileInfo> Files { get; set; }

    public DbSet<Token> Tokens { get; set; }
}
