using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    public class HttpContextServiceProviderProxy(IHttpContextAccessor contextAccessor) : IServiceProviderProxy
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));

        public T GetService<T>()
        {
            return contextAccessor.HttpContext.RequestServices.GetService<T>();
        }

        public IEnumerable<T> GetServices<T>()
        {
            return contextAccessor.HttpContext.RequestServices.GetServices<T>();
        }

        public object GetService(Type serviceType)
        {
            return contextAccessor.HttpContext.RequestServices.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return contextAccessor.HttpContext.RequestServices.GetServices(type);
        }
    }
}
