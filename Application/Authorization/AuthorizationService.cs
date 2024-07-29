using Application.Authorization.Dto;
using Main.Domain.Authorization;
using Main.Persistence.Authorization;
using Main.Security.Jwt;
using Microsoft.Extensions.Options;

namespace Application.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ITokenHelper _tokenHelper;

    private readonly TokenOptions _tokenOptions;

    public AuthorizationService(IOptions<TokenOptions> tokenOptions, IUserRepository userRepository, IUserPermissionRepository userPermissionRepository, IPermissionRepository permissionRepository, ITokenHelper tokenHelper)
    {
        _userRepository = userRepository;
        _userPermissionRepository = userPermissionRepository;
        _permissionRepository = permissionRepository;
        _tokenOptions = tokenOptions.Value;
        _tokenHelper = tokenHelper;
    }

    public async Task<User?> AddOrUpdateUser(AddOrUpdateUserInput user)
    {
        User result = null;

        if(user.Id == 0)
        {
            //result = await _userRepository.InsertAsync(new User
            //{
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    Email = user.Email,
            //    IsActive = user.IsActive,
            //    Username = user.Username,
            //    PasswordHash = [],
            //    PasswordSalt = []
            //});
        }
        else
        {
            //var dbUser = await _userRepository.GetAsync(user.Id);
            //if (dbUser is not null)
            //{
            //    dbUser.FirstName = user.FirstName;
            //    dbUser.LastName = user.LastName;
            //    dbUser.Email = user.Email;
            //    dbUser.Username = user.Username;
            //    dbUser.IsActive = user.IsActive;

            //    result = await _userRepository.UpdateAsync(dbUser);
            //}
        }

        if (result == null)
            throw new Exception();
        else
            return result;
    }

    public async Task<User?> GetById(int id)
    {
        return new User();//await _userRepository.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<User>> GetUsers()
    {
        return [];//await _userRepository.GetAll().Where(u => u.IsActive).ToListAsync();
    }

    public async Task<AuthLoginResponse?> LoginAsync(AuthLoginRequest request)
    {
        var user = new User();//await _userRepository.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            return null;

        var token = await _tokenHelper.GenerateToken(user);

        return new AuthLoginResponse(user, token);
    }
}
