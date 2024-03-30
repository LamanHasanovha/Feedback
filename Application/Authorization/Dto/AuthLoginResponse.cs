using Main.Domain.Authorization;

namespace Application.Authorization.Dto;

public class AuthLoginResponse(User user, string token)
{
    public int Id { get; set; } = user.Id;

    public string FirstName { get; set; } = user.FirstName;

    public string LastName { get; set; } = user.LastName;

    public string Username { get; set; } = user.Username;

    public string Token { get; set; } = token;
}
