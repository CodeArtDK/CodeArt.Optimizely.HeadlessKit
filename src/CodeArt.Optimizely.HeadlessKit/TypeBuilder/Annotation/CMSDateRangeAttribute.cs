namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CMSDateRangeAttribute : Attribute
    {
        public CMSDateRangeAttribute(string minimum, string maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public string Minimum { get; }

        public string Maximum { get; }
    }
}
