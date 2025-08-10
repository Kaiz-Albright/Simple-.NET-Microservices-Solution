using Company.Platform.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Company.Platform.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Platform> Platforms => Set<Platform>();
}
