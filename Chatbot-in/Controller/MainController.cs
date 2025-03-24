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

    ////Will be replaced with JWT-Authentification
    //[HttpGet("token")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<ActionResult<Token>> GetToken()
    //{
    //    _logger.Log(LogLevel.Debug, "Entered MainController.GetToken");

    //    Token t = new Token
    //    {
    //        TokenId = "Token" + Random.Shared.Next(500),
    //        CreatedAt = DateTime.Now,
    //        LastUse = DateTime.Now

    //    };

    //    await _tokenRepository.SaveToken(t);
    //    return base.Ok(t);
    //}

    //Token has to be replaced with username
    [HttpGet("files")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ContextFileInfo>>> GetFiles()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetFiles");
        //Get Fileinfos from Database

        string? token = Request.Headers["token"];
        if (token == null){
            _logger.LogWarning("No token found");
            return BadRequest("Bad Owner Token!");
        }

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

        string? filename = Request.Headers["filename"];
        string? token = Request.Headers["token"];

        //TODO: Owner Token verification
        if (token == null)
            return BadRequest("Bad Owner Token!");


        if (filename == null)
            return BadRequest("Filename missing!");

        //ownerToken will be replaced with Username in the future
        ContextFileInfo? fileInfo = await _fileHandling.SaveFile(token, filename, data);

        if (fileInfo == null)
        {
            return base.Problem("Something went wrong with saving the file. (Don't) contact the Administor for further information.");
        }

        if (await _contextFileInfoRepository.SaveFile(fileInfo) == 0)
        {
            return base.Problem("Something went wrong with saving the fileinfo. (Don't) contact the Administor for further information.");
        }

        return Created();
    }

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> DeleteFile()
    {
        string? filename = Request.Headers["filename"];
        string? token = Request.Headers["token"];

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(filename))
            return base.Problem("Wrong token or filename");

        var files = await _contextFileInfoRepository.GetFiles(token);

        if (files == null)
        {
            _logger.LogWarning("No files found for {token}", token);
            return base.NotFound();
        }

        ContextFileInfo file = files.ToList().Where(f => f.FileName.Equals(filename)).ToArray().First();

        if (file == null)
            return base.NotFound();

        await _contextFileInfoRepository.DeleteFile(file);

        var success = await _fileHandling.DeleteFile(token, filename);

        if (!success)
        {
            _logger.LogWarning($"Failed to delete file: {filename}");
            return base.BadRequest();
        }

        _logger.LogInformation("File deleted");
        return Ok("File Deleted!");
    }

    [HttpPost("chat")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> PostChat()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.Chat");
        return Ok("Beep Boop I'm a bot!");
    }

    [HttpPost("analyze")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> PostAnalyze()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.Analyze");

        return Ok("Analyzed.");
    }
}
