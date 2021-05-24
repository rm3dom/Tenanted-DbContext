namespace Example.Tenancy
{
    /// <summary>
    ///     Provides access to the tenant in the current execution context.
    ///     The current execution context could be a web request or a command.
    /// </summary>
    public interface ITenantAccessor
    {
        public TenantInfo GetCurrentTenant();
    }
}