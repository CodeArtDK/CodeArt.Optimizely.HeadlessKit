using CodeArt.Optimizely.HeadlessKit.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public interface IContentRepository
    {
        Task<List<CT>?> GetChildren<CT>(string parentkey, string[]? locale = null) where CT : class, IGraphContent;
        Task<CT?> GetContent<CT>(string key, string[]? locale = null) where CT : class, IGraphContent;
        Task<CT?> GetContentByPath<CT>(string path, string[]? locale = null) where CT : class, IGraphContent;
    }
}
