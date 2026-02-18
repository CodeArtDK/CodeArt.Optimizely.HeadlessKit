namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Sync
{
    public class SyncReport
    {
        public List<string> Created { get; } = new();
        public List<string> Updated { get; } = new();
        public List<string> Unchanged { get; } = new();
        public List<SyncFailure> Failed { get; } = new();

        public override string ToString()
        {
            return $"Created: {Created.Count}, Updated: {Updated.Count}, Unchanged: {Unchanged.Count}, Failed: {Failed.Count}";
        }
    }

    public class SyncFailure
    {
        public string Key { get; }
        public string ErrorMessage { get; }

        public SyncFailure(string key, string errorMessage)
        {
            Key = key;
            ErrorMessage = errorMessage;
        }
    }
}
