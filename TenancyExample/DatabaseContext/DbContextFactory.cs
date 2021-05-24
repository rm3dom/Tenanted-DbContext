using System;
using Example.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.DataSource
{
    public static class DbContextFactory
    {
        public static TDbContext CreateContext<TDbContext>(IServiceProvider serviceProvider)
            where TDbContext : DbContext
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var tenant = serviceProvider.GetRequiredService<ITenantAccessor>().GetCurrentTenant();

            var connectionString = configuration.GetConnectionString(tenant.Name)
                                   ?? throw new TenancyException($"No connection string for tenant: {tenant.Name}");

            var options = new DbContextOptionsBuilder<TDbContext>()
                .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)),
                    builder => { builder.EnableRetryOnFailure(3); })
                .Options;

            return Activator.CreateInstance(typeof(TDbContext), options) as TDbContext
                   ?? throw new ArgumentException("Invalid db context type");
        }
    }
}