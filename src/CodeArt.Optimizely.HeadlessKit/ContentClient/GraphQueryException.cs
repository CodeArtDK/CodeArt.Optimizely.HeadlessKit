namespace CodeArt.Optimizely.HeadlessKit.ContentClient
{
    /// <summary>
    /// Thrown when a GraphQL query returns one or more errors from the Optimizely Graph endpoint.
    /// </summary>
    public class GraphQueryException : Exception
    {
        /// <summary>
        /// Gets the list of GraphQL error messages returned by the server.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="GraphQueryException"/> with multiple error messages.
        /// </summary>
        /// <param name="errors">The GraphQL error messages.</param>
        public GraphQueryException(IEnumerable<string> errors)
            : base($"GraphQL query failed: {string.Join("; ", errors)}")
        {
            Errors = errors.ToList().AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GraphQueryException"/> with a single error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public GraphQueryException(string message) : base(message)
        {
            Errors = new List<string> { message }.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GraphQueryException"/> with a message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception that caused this error.</param>
        public GraphQueryException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = new List<string> { message }.AsReadOnly();
        }
    }
}
