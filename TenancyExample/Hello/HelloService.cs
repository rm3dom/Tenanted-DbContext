using System.Collections.Generic;
using Example.DataSource;

namespace Example.Hello
{
    public class HelloService : IHelloService
    {
        private readonly TenantedDbContext _dbContext;

        public HelloService(TenantedDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Model.Hello> GetAll()
        {
            return _dbContext.Hellos;
        }
    }
}