using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    public static class ServiceLocator
    {
        private static IServiceProviderProxy diProxy;
        public static IServiceProviderProxy Current => diProxy;

        public static void Initialize(IServiceProviderProxy proxy)
        {
            diProxy = proxy;
        }
    }
}
