namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    /// <summary>
    /// Known valid property format values for the Optimizely CMS SaaS REST API.
    /// </summary>
    public static class PropertyFormats
    {
        public const string ShortString = "shortString";
        public const string SelectOne = "selectOne";
        public const string ListOfString = "listOfString";
        public const string ImageUrl = "imageUrl";
        public const string DocumentUrl = "documentUrl";
        public const string Html = "html";

        public static readonly HashSet<string> ValidFormats = new(StringComparer.OrdinalIgnoreCase)
        {
            ShortString, SelectOne, ListOfString, ImageUrl, DocumentUrl, Html
        };
    }
}
