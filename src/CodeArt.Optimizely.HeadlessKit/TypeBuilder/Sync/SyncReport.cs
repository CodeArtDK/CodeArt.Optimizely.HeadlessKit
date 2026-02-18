namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Sync
{
    /// <summary>
    /// Results of a type synchronization operation, tracking created, updated, unchanged, and failed items.
    /// </summary>
    public class SyncReport
    {
        /// <summary>
        /// Gets the list of keys for types that were created.
        /// </summary>
        public List<string> Created { get; } = new();

        /// <summary>
        /// Gets the list of keys for types that were updated.
        /// </summary>
        public List<string> Updated { get; } = new();

        /// <summary>
        /// Gets the list of keys for types that were unchanged.
        /// </summary>
        public List<string> Unchanged { get; } = new();

        /// <summary>
        /// Gets the list of sync failures with their error messages.
        /// </summary>
        public List<SyncFailure> Failed { get; } = new();

        /// <summary>
        /// Returns a summary string with counts for each category.
        /// </summary>
        public override string ToString()
        {
            return $"Created: {Created.Count}, Updated: {Updated.Count}, Unchanged: {Unchanged.Count}, Failed: {Failed.Count}";
        }
    }

    /// <summary>
    /// Represents a failed sync operation for a single type or template.
    /// </summary>
    public class SyncFailure
    {
        /// <summary>
        /// Gets the key of the type or template that failed to sync.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the error message describing the failure.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncFailure"/> class.
        /// </summary>
        /// <param name="key">The key of the type or template that failed.</param>
        /// <param name="errorMessage">The error message.</param>
        public SyncFailure(string key, string errorMessage)
        {
            Key = key;
            ErrorMessage = errorMessage;
        }
    }
}
