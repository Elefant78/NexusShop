
namespace NexusShop.WebAPI.Security;

public interface ITokenService
{
    string GenerateToken(DemoUser user);
}
