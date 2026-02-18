using CodeArt.Optimizely.HeadlessKit.Core;
using Microsoft.Extensions.Options;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public class DefaultGraphQueryProvider : IGraphQueryProvider
    {
        private readonly IOptions<ContentGraphOptions> _options;
        private readonly AutoGraphQueryProvider? _autoProvider;

        public DefaultGraphQueryProvider(IOptions<ContentGraphOptions> options, AutoGraphQueryProvider? autoProvider = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _autoProvider = autoProvider;
        }

        public string ProvideMainRelativePathQuery()
        {
            if (!string.IsNullOrWhiteSpace(_options.Value?.MainQuery))
                return _options.Value.MainQuery;

            if (_autoProvider != null)
                return _autoProvider.ProvideMainRelativePathQuery();

            throw new InvalidOperationException(
                "No MainQuery configured and no AutoGraphQueryProvider available. " +
                "Either configure ContentGraphOptions.MainQuery or register IContentTypeRegistry.");
        }
    }
}
