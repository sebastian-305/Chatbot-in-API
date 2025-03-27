using Chatbot_in.Domain.Identity;
using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controller
{
    [Route("/")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ITokenService tokenService, ILogger<AuthenticateController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _logger = logger;
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
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User logged out");

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
