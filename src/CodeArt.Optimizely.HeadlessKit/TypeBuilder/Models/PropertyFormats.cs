namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models
{
    /// <summary>
    /// Known valid property format values for the Optimizely CMS SaaS REST API.
    /// </summary>
    public static class PropertyFormats
    {
        /// <summary>
        /// Single-line text input.
        /// </summary>
        public const string ShortString = "shortString";

        /// <summary>
        /// Dropdown selection. Use with <see cref="Annotation.CMSPropertyChoiceAttribute"/> to define selectable values.
        /// </summary>
        public const string SelectOne = "selectOne";

        /// <summary>
        /// Multi-value string list.
        /// </summary>
        public const string ListOfString = "listOfString";

        /// <summary>
        /// Image picker returning a URL.
        /// </summary>
        public const string ImageUrl = "imageUrl";

        /// <summary>
        /// Document or file picker returning a URL.
        /// </summary>
        public const string DocumentUrl = "documentUrl";

        /// <summary>
        /// Rich text HTML editor.
        /// </summary>
        public const string Html = "html";

        /// <summary>
        /// The set of all valid format values, for validation purposes.
        /// </summary>
        public static readonly HashSet<string> ValidFormats = new(StringComparer.OrdinalIgnoreCase)
        {
            ShortString, SelectOne, ListOfString, ImageUrl, DocumentUrl, Html
        };
    }
}
