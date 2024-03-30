using Application.Authorization;
using Main.Security.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebAPI.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenOptions _tokenOptions;

    public JwtMiddleware(RequestDelegate next, IOptions<TokenOptions> tokenOptions)
    {
        _next = next;
        _tokenOptions = tokenOptions.Value;
    }

    public async Task Invoke(HttpContext context, IAuthorizationService service)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split().Last();

        if (token != null)
            await AttachUserToContext(context, service, token);

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, IAuthorizationService service, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenOptions.SecurityKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            context.Items["User"] = await service.GetById(userId);
        }
        catch (Exception ex)
        {

        }
    }
}
