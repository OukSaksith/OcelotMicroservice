
using Microsoft.EntityFrameworkCore;
using OcelotGateway.Model;

namespace OcelotGateway;

public class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
}
