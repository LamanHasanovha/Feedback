using Application.Authorization.Dto;
using Main.Domain.Authorization;

namespace Application.Authorization;

public interface IAuthorizationService
{
    Task<AuthLoginResponse?> LoginAsync(AuthLoginRequest request);

    Task<List<User>> GetUsers();

    Task<User?> GetById(int id);

    Task<User?> AddOrUpdateUser(AddOrUpdateUserInput user);
}
