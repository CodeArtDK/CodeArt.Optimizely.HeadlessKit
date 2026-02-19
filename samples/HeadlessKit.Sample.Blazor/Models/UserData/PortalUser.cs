namespace HeadlessKit.Sample.Blazor.Models.UserData
{
    public class PortalUser
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Plan { get; set; } = "";
        public DateTime MemberSince { get; set; }
        public Dictionary<string, int> UsageData { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
    }

    public class Invoice
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public string Status { get; set; } = "";
    }
}
