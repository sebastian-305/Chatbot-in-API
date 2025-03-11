using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controllers;


[ApiController]
[Route("/")]
public class MainController : ControllerBase
{

    private readonly ILogger<MainController> _logger;
    private readonly ITokenRepository _tokenRepository;
 

    public MainController(ILogger<MainController> logger, ITokenRepository tokenRepository)
    {
        _logger = logger;
        _tokenRepository = tokenRepository;
    }

    [HttpGet("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Token>> GetToken()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetToken");


        Token t = new Token
        {
            TokenId = "Token" + Random.Shared.Next(500),
            CreatedAt = DateTime.Now,
            LastUse = DateTime.Now
          
        };

        await _tokenRepository.SaveToken(t);
        return Ok(t);
    }

    [HttpGet("files")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ContextFileInfo>>> GetFiles()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetFiles");
        //Get Fileinfos from Database

        return Ok();
    }

    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> PostFile()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.PostFile");
        return Ok("File Uploaded! (Not really)");
    }

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> DeleteFile()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.DeleteFile");
        return Ok("File Deleted! (Not really)");
    }

    [HttpPost("chat")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> PostChat()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.Chat");
        return Ok("Beep Boop I'm a bot!");
    }
}
