using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using System.Web.Http.Dependencies;
using Spring.Context;

namespace CoinExchange.Rest.WebHost
{
    /// <summary>
    /// SpringDependencyResolver required for Spring Dependency Injection
    /// </summary>
    public class SpringDependencyResolver : IDependencyResolver
    {
        private readonly IApplicationContext _context;

        public SpringDependencyResolver(IApplicationContext context)
        {
            _context = context;
        }

        public object GetService(Type serviceType)
        {
            var dictionary = _context.GetObjectsOfType(serviceType).GetEnumerator();

            dictionary.MoveNext();
            try
            {
                return dictionary.Value;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _context.GetObjectsOfType(serviceType).Cast<object>();
        }
    }
}