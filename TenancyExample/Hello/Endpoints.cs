using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Hello
{
    public static class Endpoints
    {
        public static IEndpointConventionBuilder AddHelloEndpoints(this IEndpointRouteBuilder enpoints)
        {
            return enpoints.MapGet("/api/hello", async context =>
            {
                var helloSrv = context.RequestServices.GetRequiredService<IHelloService>();
                await context.Response.WriteAsJsonAsync(helloSrv.GetAll());
            });
        }
    }
}