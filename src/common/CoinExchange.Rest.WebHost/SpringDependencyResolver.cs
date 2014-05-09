using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using Spring.Context;
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace CoinExchange.Rest.WebHost
{
    public class SpringDependencyResolver:IDependencyResolver
    {
        private readonly IApplicationContext _context;

        public SpringDependencyResolver(IApplicationContext context)
        {
            _context = context;
        }
        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            var dictionary = _context.GetObjectsOfType(serviceType).GetEnumerator();

            dictionary.MoveNext();
            try
            {
                return _context[serviceType.Name];
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _context.GetObjectsOfType(serviceType).Cast<object>();
        }

        public void Dispose()
        {
        }
    }
}