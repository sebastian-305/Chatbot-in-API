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
    public string GetToken()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetToken");
        return "Here is your token!";
    }

    [HttpGet("files")]
    public string GetFiles()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetFiles");
        return "No files here!";
    }

    [HttpPost("upload")]
    public string PostFile()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.PostFile");
        return "File Uploaded! (Not really)";
    }

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public string DeleteFile()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.DeleteFile");
        return "File Deleted! (Not really)";
    }

    [HttpPost("chat")]
    public string PostChat()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.Chat");
        return "Beep Boop I'm a bot!";
    }
}
