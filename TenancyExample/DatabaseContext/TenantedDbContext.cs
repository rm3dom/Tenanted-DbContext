using Microsoft.EntityFrameworkCore;

namespace Example.DataSource
{
    public class TenantedDbContext : DbContext
    {
        public TenantedDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Hello.Model.Hello> Hellos { get; set; } = null!;
    }
}