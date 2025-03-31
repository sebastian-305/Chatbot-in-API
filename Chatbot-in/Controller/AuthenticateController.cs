using System.Runtime.CompilerServices;
using Chatbot_in.Domain.Identity;
using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;
using Chatbot_in.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controller
{
    [Route("/")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IBlackListRepository _blackList;

        public AuthenticateController(UserManager<ApplicationUser> userManager, ITokenService tokenService, ILogger<AuthenticateController> logger, IBlackListRepository blackList)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
            _blackList = blackList;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Ok(await _tokenService.CreateTokenAsync(user));
            }
            return Unauthorized();
        }


        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User logged out");
            try
            {
                var expire = new DateTime(Convert.ToInt64(Request.HttpContext.User.Claims.First(c => c.Type == "exp").Value));
                var token = Request.Headers.Authorization.ToString().Split(" ")[1];
                _logger.LogDebug("Token: " + token);
                _logger.LogDebug("Expires: " + expire.ToString());
                await _blackList.AddTokenAsync(new BlackListTokenDTO() { Token = token, Expiration = expire });
            }
            catch (Exception ex)
            {
                _logger.LogError("{message}", ex.Message);
                return BadRequest();
            }

            return Ok(new ResponseDTO { Status = "Success", Message = "Logged out successfully" });
        }


        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseDTO { Status = "Error", Message = "User already exists!" });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded){
                    foreach (var error in result.Errors)
                        _logger.LogError("{code}: {message}",error.Code, error.Description);
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseDTO { Status = "Error", Message = result.Errors.First().Description});
}                await _userManager.AddToRoleAsync(user, "User");

                return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogCritical("{e}",ex.Message);
                return StatusCode(500);
            }
        }

    }
}
