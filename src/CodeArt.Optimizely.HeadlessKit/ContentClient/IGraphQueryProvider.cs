using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public interface IGraphQueryProvider
    {
        /// <summary>
        /// Provides a GraphQL Query that takes a relative path and locales
        /// </summary>
        /// <returns></returns>
        public string ProvideMainRelativePathQuery();


        //Introspection Query

        //GetContentFromKey query

        //GetChildren query

    }
}
