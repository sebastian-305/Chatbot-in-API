using Chatbot_in.Domain.Identity;
using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ITokenService tokenService, ILogger<AuthenticateController> logger)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Ok(await tokenService.CreateTokenAsync(user));
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try
            {
                var userExists = await userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseDTO { Status = "Error", Message = "User already exists!" });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                foreach (var er in result.Errors)
                {
                    Console.WriteLine(er);
                }

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseDTO { Status = "Error", Message = "User creation failed! Please check user details and try again." });
                await userManager.AddToRoleAsync(user, "User");

                return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("{e}",ex.Message);
                return StatusCode(500);
            }
        }

    }
}
