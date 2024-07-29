using Main.Domain.Authorization;

namespace Main.Security.Jwt;

public interface ITokenHelper
{
    Task<string> GenerateToken(User user);
}
