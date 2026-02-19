using System.Text.Json;
using HeadlessKit.Sample.Blazor.Models.UserData;

namespace HeadlessKit.Sample.Blazor.Services
{
    public class PortalUserService
    {
        private readonly List<PortalUser> _users;

        public PortalUserService(IWebHostEnvironment env)
        {
            var path = Path.Combine(env.ContentRootPath, "Data", "users.json");
            var json = File.ReadAllText(path);
            _users = JsonSerializer.Deserialize<List<PortalUser>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();
        }

        public PortalUser? Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)
                && u.Password == password);
        }

        public PortalUser? GetByUsername(string username)
        {
            return _users.FirstOrDefault(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
