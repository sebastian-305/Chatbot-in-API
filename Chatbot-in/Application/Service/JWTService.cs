using Chatbot_in.Domain.Identity;
using Chatbot_in.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chatbot_in.Application.Service;

public class JWTService : ITokenService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IConfiguration configuration;
    private readonly ILogger<JWTService> _logger;
    public JWTService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<JWTService> logger)
    {
        this.userManager = userManager;
        this.configuration = configuration;
        _logger = logger;
    }

    public async Task<TokenDTO> CreateTokenAsync(ApplicationUser user)
    {

        var userRoles = await userManager.GetRolesAsync(user);
        //IList<string>? userRoles = new List<string>();
        //userRoles.Add("User");
        _logger.LogDebug("User Roles: {roles}", userRoles.Count);


        var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


        foreach (string userRole in userRoles)
        {
            _logger.LogInformation("User `{user}` claims role `{role}`", user.UserName, userRole);
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }


        var secret = configuration.GetSection("JwtSettings")["Secret"]!;
        Console.WriteLine("Generation Secret:" + secret);
        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddDays(15),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        return new TokenDTO()
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            roles = userRoles,
            id = user.Id
        };
    }

}