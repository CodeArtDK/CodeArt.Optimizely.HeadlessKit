using CodeArt.Optimizely.HeadlessKit.Core.Models;

namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public class GraphQueryResult<T> where T : class, IGraphContent
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public string? Cursor { get; set; }
    }
}
