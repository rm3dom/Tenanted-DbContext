using System.Linq;
using System.Threading.Tasks;
using Example.Tenancy;
using Microsoft.AspNetCore.Http;

namespace Example
{
    public interface IMiddleware
    {
        Task Invoke(HttpContext context);
    }

    public class TenantMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var req = context.Request;
            var tenantName = req.Headers["X-TenantId"]
                                 .SingleOrDefault()
                             ?? GetTenantFromHost(req)
                             ?? throw new TenancyException("Unable to get host from request");

            AsyncLocalTenantAccessor.SetTenant(new TenantInfo
            {
                Name = tenantName
            });

            return _next.Invoke(context);
        }

        private string? GetTenantFromHost(HttpRequest req)
        {
            if (req.Host.HasValue)
                return req.Host.Host;
            return null;
        }
    }
}