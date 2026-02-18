using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Attributes
{
    /// <summary>
    /// Registers a Razor Page, MVC Controller, or ViewComponent as the rendering template
    /// for a specific content type. Applied to PageModel, Controller, or ViewComponent classes.
    /// </summary>
    /// <example>
    /// <code>
    /// [TemplateDescriptor(typeof(StandardPage))]
    /// public class StandardPageModel : ContentPage&lt;StandardPage&gt;
    /// {
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TemplateDescriptorAttribute : Attribute
    {
        /// <summary>
        /// The content type this template renders.
        /// </summary>
        public Type TemplateFor { get; set; }

        /// <summary>
        /// Optional alternate template identifier. When specified, this template is only used
        /// when the matching rendering tag is requested.
        /// </summary>
        public string? RenderingTag { get; set; }

        /// <summary>
        /// Whether this template can be used when no specific rendering tag is requested.
        /// Defaults to <c>true</c>.
        /// </summary>
        public bool AvailableWithoutTag { get; set; }

        /// <summary>
        /// Whether this template also handles derived content types. Defaults to <c>true</c>.
        /// </summary>
        public bool Inherited { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateDescriptorAttribute"/> class.
        /// </summary>
        /// <param name="GraphContentType">The content type this template renders.</param>
        public TemplateDescriptorAttribute(Type GraphContentType)
        {
            TemplateFor = GraphContentType;
            AvailableWithoutTag = true;
            Inherited = true;
        }
    }
}
