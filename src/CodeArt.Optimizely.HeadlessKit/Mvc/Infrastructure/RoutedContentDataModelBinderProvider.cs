using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    public class RoutedContentDataModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return context.Metadata.ModelType.IsAssignableTo(typeof(IGraphContent)) ? new RoutedContentDataModelBinder() : (IModelBinder)null;

        }
    }
}
