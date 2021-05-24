using System.Collections.Generic;

namespace Example.Hello
{
    public interface IHelloService
    {
        IEnumerable<Model.Hello> GetAll();
    }
}