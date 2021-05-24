using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Example.Tenancy
{
    public static class TenancyUtil
    {
        /// <summary>
        ///     Finds the tenant name from a request.
        ///     Looks at headers and host.
        /// </summary>
        public static string TenantName(this HttpRequest request)
        {
            return request.Headers["X-TenantId"]
                       .SingleOrDefault()
                   ?? GetTenantFromHost(request)
                   ?? throw new TenancyException("Unable to get host from request");
        }

        /// <summary>
        ///     Get the tenant from a domain name, ie: tenant0.example.com.
        /// </summary>
        private static string? GetTenantFromHost(HttpRequest req)
        {
            if (req.Host.HasValue)
            {
                var host = req.Host.Host;
                var dot = host.IndexOf('.');
                if (dot < 2)
                    return host;
                return host.Substring(0, dot);
            }

            return null;
        }
    }
}