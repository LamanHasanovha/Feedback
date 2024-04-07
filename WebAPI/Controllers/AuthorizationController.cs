using Application.Authorization;
using Application.Authorization.Dto;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Attributes;

namespace WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _authorizationService.GetUsers());
    }

    [HttpPost("AddOrUpdateUser")]
    [Authorize]
    public async Task<IActionResult> AddOrUpdateUser([FromBody] AddOrUpdateUserInput request)
    {
        return Ok(await _authorizationService.AddOrUpdateUser(request));
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        var response = await _authorizationService.LoginAsync(request);

        if (response == null)
            return BadRequest(new { Message = "Username or password is incorrect!" });

        return Ok(response);
    }
}
