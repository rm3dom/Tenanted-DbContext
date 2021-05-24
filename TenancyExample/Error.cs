using System;

namespace Example
{
    public class TenancyException : ApplicationException
    {
        public TenancyException(string? message) : base(message)
        {
        }
    }
}