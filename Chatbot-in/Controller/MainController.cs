using Chatbot_in.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controllers;


[ApiController]
[Route("/")]
public class MainController : ControllerBase
{

    private readonly ILogger<MainController> _logger;
 

    public MainController(ILogger<MainController> logger)
    {
        _logger = logger;
    }

    [HttpGet("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Token>> GetToken()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetToken");

        //Token generation
        
        return Ok(new Token().TokenId = "Not a real Token");
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
