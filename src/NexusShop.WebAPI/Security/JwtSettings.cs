namespace NexusShop.WebAPI.Security;

/// <summary>Strongly-typed JWT configuration bound from appsettings.json.</summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; } = 60;
}
