using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    public interface IServiceProviderProxy
    {
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
        object GetService(Type serviceType);
        IEnumerable<object> GetServices(Type serviceType);
    }
}
