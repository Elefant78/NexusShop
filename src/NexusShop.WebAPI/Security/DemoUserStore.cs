namespace NexusShop.WebAPI.Security;

/// <summary>Holds the demo users used by the AuthController.</summary>
public sealed class DemoUserStore
{
    private readonly IReadOnlyList<DemoUser> _users = new List<DemoUser>
    {
        new("admin", "Admin123!", "Admin"),
        new("customer", "Customer123!", "Customer")
    };

    public DemoUser? Find(string username, string password) =>
        _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
}
