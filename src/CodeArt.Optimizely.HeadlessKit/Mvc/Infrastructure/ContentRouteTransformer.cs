using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    /// <summary>
    /// ASP.NET Core <see cref="DynamicRouteValueTransformer"/> that resolves CMS content from incoming URLs.
    /// Fetches content from Optimizely Graph by path and maps the result to the appropriate
    /// Razor Page or Controller using <see cref="TemplateCoordinator"/>.
    /// </summary>
    /// <remarks>
    /// Handles both regular content requests (resolved by URL path) and CMS preview requests
    /// (via the <c>/preview</c> endpoint with <c>key</c>, <c>ver</c>, and <c>preview_token</c>
    /// query parameters). The resolved content is stored in <c>HttpContext.Items["CurrentContent"]</c>.
    /// </remarks>
    public class ContentRouteTransformer : DynamicRouteValueTransformer
    {
        private readonly ContentGraphClient _contentGraphClient;
        private readonly TemplateCoordinator _templateCoordinator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentRouteTransformer"/> class.
        /// </summary>
        /// <param name="contentGraphClient">The Graph client for fetching content by path or key.</param>
        /// <param name="templateCoordinator">The coordinator for resolving content types to rendering templates.</param>
        public ContentRouteTransformer(ContentGraphClient contentGraphClient, TemplateCoordinator templateCoordinator)
        {
            _contentGraphClient = contentGraphClient;
            _templateCoordinator = templateCoordinator;
        }

        /// <inheritdoc />
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            //If we have content, lookup the rendering controller for the content item
            IGraphContent? content = null;
            if (httpContext.Request.Path.Value == "/preview")
            {
                //We have a preview request
                string key = httpContext.Request.Query["key"].ToString();
                string ver = httpContext.Request.Query["ver"].ToString();
                string loc = httpContext.Request.Query["loc"].ToString();
                string ctx = httpContext.Request.Query["ctx"].ToString();
                string preview_token = httpContext.Request.Query["preview_token"].ToString();
                content = await _contentGraphClient.GetPreviewContentByKey<GraphContent>(key, ver, preview_token);
                httpContext.Items["PreviewMode"] = ctx;
                httpContext.Items["PreviewToken"] = preview_token;
                httpContext.Items["ContentKey"] = key;
            }
            else
            {
                //We have a normal request
                content = await _contentGraphClient.GetContentByPath<GraphContent>(httpContext.Request.Path.Value);
            }
            if (content != null)
            {
                //Logic to determine which controller or RazorPage to use for which contenttype
                var template = _templateCoordinator.GetTemplateMappingInfo(content.GetType());
                if (template != null && template.IsController)
                {
                    httpContext.Items["CurrentContent"] = content;

                    values["controller"] = template.TemplateType.Name.Replace("Controller", "");
                    values["action"] = "Index";
                    values["CurrentContent"] = content;
                    return values;
                }

                var templateName = _templateCoordinator.GetTemplateEndpointForType(content.GetType());

                if (templateName != null)
                {
                    httpContext.Items.Add("CurrentContent", content);

                    values["page"] = templateName;

                    values["CurrentContent"] = content;


                }
                else
                {
                    ////If internal then this might be a block with no endpoint? Setup sample endpoint
                    //if (httpContext.IsPreview() || httpContext.IsOnPageEdit())
                    //{
                    //    httpContext.Items.Add("CurrentContent", content);
                    //    values["page"] = "/BlockPreviewPage";
                    //    values["CurrentContent"] = content;
                    //}
                    //else
                    //{
                    //    //TODO: Else? throw error
                    //    throw new ApplicationException($"No Page available to render {content.GetType().Name}");
                    //}
                }
            }
            else
            {
                //Maybe 404? or fallback
            }

            return values;
        }
    }
}
