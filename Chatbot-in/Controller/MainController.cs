using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;
using Chatbot_in.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controllers;


[ApiController]
[Route("/")]
public class MainController : ControllerBase
{
    private readonly ILogger<MainController> _logger;
    private readonly IFileHandlingTemporary _fileHandling;
    private readonly IContextFileInfoRepository _contextFileInfoRepository;
    private readonly GeminiService _geminiService;

    public MainController(ILogger<MainController> logger, IFileHandlingTemporary fileHandling, IContextFileInfoRepository contextFileInfoRepository, GeminiService geminiService)
    {
        _logger = logger;
        _fileHandling = fileHandling; //Speichert, lädt oder löscht Dateien im Dateisystem von Hetzner
        _contextFileInfoRepository = contextFileInfoRepository; //Speichert Dateiname und Username in Hetzner-Datenbank
        _geminiService = geminiService;
    }


    //Get Fileinfos from Database
    [HttpGet("files")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<IEnumerable<ContextFileInfo>>> GetFiles()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetFiles");
        
        string short_id = GetShortUid(Request);

        var files = await _contextFileInfoRepository.GetFiles(short_id);

        return Ok(files);
    }

    [HttpPost("upload")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<string>> PostFile()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.PostFile");

        string short_id = GetShortUid(Request);

        byte[] data; //Hier werden die Daten der Datei gespeichert
        using (var ms = new MemoryStream(2048))
        {
            await Request.Body.CopyToAsync(ms);
            data = ms.ToArray();//Daten der Datei werden im Byte-Array gespeichert
        }

        string? filename = Request.Headers["filename"];

        if (short_id == String.Empty)
            return BadRequest("Bad Token!");

        if (filename == null)
            return BadRequest("Filename missing!");

        ContextFileInfo? fileInfo = await _fileHandling.SaveFile(short_id, filename, data);

        if (fileInfo == null)
        {
            return base.Problem("Something went wrong with saving the file. (Don't) contact the Administor for further information.");
        }

        if (await _contextFileInfoRepository.SaveFile(fileInfo) == 0)
        {
            return base.Problem("Something went wrong with saving the fileinfo. (Don't) contact the Administor for further information.");
        }

        return Accepted();
    }

    [HttpDelete("delete")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<string>> DeleteFile()
    {
        _logger.LogInformation("Entering MainController.DeleteFile()");
        string? filename = Request.Headers["filename"];
        string short_id = GetShortUid(Request);

        _logger.LogDebug("Filename: {file}, token: {token}", filename, short_id);


        if (string.IsNullOrWhiteSpace(short_id) || string.IsNullOrWhiteSpace(filename))
        {
            _logger.LogWarning("Wrong token {token} or filename {filename}", short_id, filename);
            return base.Problem("Wrong token or filename");
        }

        var files = await _contextFileInfoRepository.GetFiles(short_id);

        if (files == null)
        {
            _logger.LogWarning("No files found for {token}", short_id);
            return base.NotFound();
        }

        ContextFileInfo file = files.ToList().Where(f => f.FileName.Equals(filename)).ToArray().First();

        if (file == null)
            return base.NotFound();

        await _contextFileInfoRepository.DeleteFile(file);

        var success = await _fileHandling.DeleteFile(short_id, filename);

        if (!success)
        {
            _logger.LogWarning($"Failed to delete file: {filename}");
            return base.BadRequest();
        }

        _logger.LogInformation("File deleted");
        return Ok("File Deleted!");
    }

    [HttpPost("chat")]
    public async Task<ActionResult<string>> PostChat(string prompt)
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.Chat");
        string response = await _geminiService.ChatWithAI(prompt);
        return Ok(new { response });
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<string>> PostAnalyze()
    {
        string response = await _geminiService.InitializeAi("t");
        return Ok(new { response });
    }

    private string GetShortUid(HttpRequest request)
    {
        try
        {
            var userid = request.HttpContext.User.Claims.First(c => c.Type == "sub").Value;
            _logger.LogDebug("User: {id}", userid);

            string short_id = userid.Split('-').Last() ?? String.Empty;

            _logger.LogDebug("Short uid: {suid}", short_id);
            return short_id;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return String.Empty;
        }
    }
}
