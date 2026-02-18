using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    public class RoutedContentDataModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var content = bindingContext.HttpContext.Items["CurrentContent"];

            if (content != null && bindingContext.ModelType.IsInstanceOfType(content))
            {
                bindingContext.Result = ModelBindingResult.Success(content);
            }

            return Task.CompletedTask;
        }
    }
}
