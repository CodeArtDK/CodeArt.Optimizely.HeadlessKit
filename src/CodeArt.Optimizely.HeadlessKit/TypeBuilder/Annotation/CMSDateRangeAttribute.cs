namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    /// <summary>
    /// Restricts a DateTime property to a specific date range in the CMS editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CMSDateRangeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CMSDateRangeAttribute"/> class.
        /// </summary>
        /// <param name="minimum">The earliest allowed date as an ISO date string (e.g. "2020-01-01").</param>
        /// <param name="maximum">The latest allowed date as an ISO date string (e.g. "2030-12-31").</param>
        public CMSDateRangeAttribute(string minimum, string maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        /// <summary>
        /// Gets the earliest allowed date as an ISO date string.
        /// </summary>
        public string Minimum { get; }

        /// <summary>
        /// Gets the latest allowed date as an ISO date string.
        /// </summary>
        public string Maximum { get; }
    }
}
