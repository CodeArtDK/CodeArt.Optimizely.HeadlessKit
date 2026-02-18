using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public class GraphQLHttpRequestWithAuthSupport : GraphQLHttpRequest
    {
        public AuthenticationHeaderValue Authentication { get; set; }

        public override HttpRequestMessage ToHttpRequestMessage(GraphQLHttpClientOptions options, IGraphQLJsonSerializer serializer)
        {
            var r = base.ToHttpRequestMessage(options, serializer);
            r.Headers.Authorization = Authentication;
            return r;
        }
    }
}
