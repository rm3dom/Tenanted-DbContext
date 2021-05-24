using System.Threading;

namespace Example.Tenancy
{
    public class AsyncLocalTenantAccessor : ITenantAccessor
    {
        //Should be a stack but for the example this is enough.
        private static readonly AsyncLocal<TenantInfo> Ctx = new();

        public TenantInfo GetCurrentTenant()
        {
            return Ctx.Value ?? throw new TenancyException("No/Null tenant!");
        }

        public static void SetTenant(TenantInfo info)
        {
            Ctx.Value = info;
        }
    }
}