using System;
using System.IO;
using System.Linq;
using Example.DataSource;
using Example.Hello;
using Example.Tenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddCommandLine(Environment.GetCommandLineArgs());

            Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.json")
                .ToList()
                .ForEach(file => configBuilder.AddJsonFile(file, false, true));

            services.AddLogging(logBuilder => { logBuilder.SetMinimumLevel(LogLevel.Debug); })
                .AddSingleton<IConfiguration>(configBuilder.Build())
                .AddScoped<ITenantAccessor, AsyncLocalTenantAccessor>()
                .AddScoped<IHelloService, HelloService>()
                .AddScoped(DbContextFactory.CreateContext<TenantedDbContext>);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<TenantMiddleware>();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.AddHelloEndpoints(); });
        }
    }
}