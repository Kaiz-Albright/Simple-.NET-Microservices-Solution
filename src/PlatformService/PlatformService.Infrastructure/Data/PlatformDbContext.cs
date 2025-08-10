using Microsoft.EntityFrameworkCore;
using PlatformService.Domain.Entities;

namespace PlatformService.Infrastructure.Data;

public class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options)
    {
    }

    public DbSet<Platform> Platforms => Set<Platform>();
}
