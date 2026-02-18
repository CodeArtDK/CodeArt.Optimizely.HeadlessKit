namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    public class GraphQueryException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public GraphQueryException(IEnumerable<string> errors)
            : base($"GraphQL query failed: {string.Join("; ", errors)}")
        {
            Errors = errors.ToList().AsReadOnly();
        }

        public GraphQueryException(string message) : base(message)
        {
            Errors = new List<string> { message }.AsReadOnly();
        }

        public GraphQueryException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = new List<string> { message }.AsReadOnly();
        }
    }
}
