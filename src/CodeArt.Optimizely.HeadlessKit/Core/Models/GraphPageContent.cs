using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Base class for page content types. Extends <see cref="GraphContent"/> with page-specific behavior.
    /// Pages are routable content items that have their own URLs.
    /// </summary>
    public class GraphPageContent : GraphContent, IGraphPageContent
    {

    }
}
