using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;
using Chatbot_in.Domain.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controllers;


[ApiController]
[Route("/")]
public class MainController : ControllerBase
{

    private readonly ILogger<MainController> _logger;
    private readonly ITokenRepository _tokenRepository;
    private readonly IFileHandlingTemporary _fileHandling;
    private readonly IContextFileInfoRepository _contextFileInfoRepository;


    public MainController(ILogger<MainController> logger, ITokenRepository tokenRepository, IFileHandlingTemporary fileHandling, IContextFileInfoRepository contextFileInfoRepository)
    {
        _logger = logger;
        _tokenRepository = tokenRepository;
        _fileHandling = fileHandling;   
        _contextFileInfoRepository = contextFileInfoRepository;
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

    [HttpGet("files/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ContextFileInfo>>> GetFiles(string token)
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetFiles");
        //Get Fileinfos from Database

        var files = await _contextFileInfoRepository.GetFiles(token);

        return Ok(files);
    }

    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> PostFile()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.PostFile");

        byte[] data;
        using (var ms = new MemoryStream(2048))
        {
            await Request.Body.CopyToAsync(ms);
            data = ms.ToArray();
        }

        string? filename = Request.Headers["Filename"];
        string? ownerToken = Request.Headers["OwnerToken"];

        //TODO: Owner Token verification
        if (ownerToken == null)
            return BadRequest("Bad Owner Token!");


        if (filename == null)
            return BadRequest("Filename missing!");

        ContextFileInfo? fileInfo = await _fileHandling.SaveFile(filename, ownerToken, data);

        if(fileInfo == null)
        {
            return base.Problem("Something went wrong with saving the file. (Don't) contact the Administor for further information.");
        }

        if (await _contextFileInfoRepository.SaveFile(fileInfo) == 0)
        {
            return base.Problem("Something went wrong with saving the fileinfo. (Don't) contact the Administor for further information.");
        }
        
        return Ok();
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
