using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TemplateDescriptorAttribute : Attribute
    {
        /// <summary>
        /// The type of the GraphContent this is a template for
        /// </summary>
        public Type TemplateFor { get; set; }

        public string? RenderingTag { get; set; }

        public bool AvailableWithoutTag { get; set; }

        public bool Inherited { get; set; }



        public TemplateDescriptorAttribute(Type GraphContentType)
        {
            TemplateFor = GraphContentType;
            AvailableWithoutTag = true;
            Inherited = true;
        }
    }
}
