namespace NexusShop.WebAPI.Security;

/// <summary>
/// A minimal in-memory user record. In a real system this would come from
/// ASP.NET Core Identity or an external identity provider; here it keeps the
/// sample self-contained while still demonstrating role-based authorization.
/// </summary>
public sealed record DemoUser(string Username, string Password, string Role);
