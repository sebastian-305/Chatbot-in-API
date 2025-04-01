using Chatbot_in.Application.Repositories;
using Chatbot_in.Application.Service;
using Chatbot_in.Domain.Models;
using Chatbot_in.Infrastructure.SignalR;
using Chatbot_in.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static System.Net.Mime.MediaTypeNames;

namespace Chatbot_in.Controllers;


[ApiController]
[Route("/")]
public class MainController : ControllerBase
{
    private readonly ILogger<MainController> _logger;
    private readonly IFileHandlingTemporary _fileHandling;
    private readonly IContextFileInfoRepository _contextFileInfoRepository;
    private readonly IBlackListRepository _blackListRepository;
    private readonly IAiService _geminiService;
    private readonly IPromptCountRepository _promptCounterService;
    private readonly IHubContext<RequestCounterHub, ISubscribeCounterClient> _counterHub;


    public MainController(ILogger<MainController> logger, IFileHandlingTemporary fileHandling,
        IContextFileInfoRepository contextFileInfoRepository, IAiService geminiService,
        IPromptCountRepository promptCounterService,
        IHubContext<RequestCounterHub, ISubscribeCounterClient> counterHub,
        IBlackListRepository blackListRepository

        )
    {
        _logger = logger;
        _fileHandling = fileHandling; //Speichert, lädt oder löscht Dateien im Dateisystem von Hetzner
        _contextFileInfoRepository = contextFileInfoRepository; //Speichert Dateiname und Username in Hetzner-Datenbank
        _geminiService = geminiService;
        _promptCounterService = promptCounterService;
        _counterHub = counterHub;
        _blackListRepository = blackListRepository;

    }

    [Authorize(Roles = "User")]
    [HttpPatch("refreshcount")]
    public async Task GetRefreshedCount()
    {

        _logger.Log(LogLevel.Debug, "Entered MainController.RefreshCount");
        if (await CheckBlackList(Request))
            return;


        await _counterHub.Clients.All.RequestCounterChanged(
        new ISubscribeCounterClient.RequestsLeftParams()
        {
            PromptsLeftDay = _promptCounterService.PromptsLeftDay,
            PromptsLeftMinute = _promptCounterService.PromptsLeftMinute,
        });

        _logger.LogInformation("Total prompst: {prompts}", _promptCounterService.TotalPrompts);
    }

    //Get Fileinfos from Database
    [HttpGet("filelist")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<IEnumerable<ContextFileInfo>>> GetFiles()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.GetFiles");
        if (await CheckBlackList(Request))
            return Unauthorized();

        string short_id = GetShortUid(Request);

        var files = await _contextFileInfoRepository.GetFiles(short_id);
        var files_filtered = files.Select(o => o.FileName).ToArray() ?? [];
        _logger.LogDebug(string.Join(", ", files_filtered));
        return Ok(files_filtered);
    }

    [HttpPost("upload")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<string>> PostFile()
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.PostFile");
        if (await CheckBlackList(Request))
            return Unauthorized();

        string short_id = GetShortUid(Request);
        string? filename = Request.Headers["filename"];
        var totalFiles = await _contextFileInfoRepository.GetFiles(short_id);

        if (totalFiles.Count() >= 3)
            return BadRequest("The Maximum of Files you can upload is 3. Delete a File to replace it.");

        if (filename?.Split('.').Last() != "txt")
            return BadRequest($"Wrong file type! Only txt files are allowed!");

        byte[] data; //Hier werden die Daten der Datei gespeichert
        using (var ms = new MemoryStream(2048))
        {
            await Request.Body.CopyToAsync(ms);
            data = ms.ToArray();//Daten der Datei werden im Byte-Array gespeichert
        }

        int size = data.Length;
        string sizeString = size.ToString("#,##0").Replace(",", " ");

        _logger.LogInformation("Size of the uploaded file is {size} bytes", sizeString);

        if (size > 40_000)
            return BadRequest($"File size is too big! Maximum Filesize is 40 000 bytes and your file has {sizeString} bytes");

        if (short_id == String.Empty)
            return BadRequest("Bad Token!");

        if (filename == null)
            return BadRequest("Filename missing!");

        ContextFileInfo? fileInfo = await _fileHandling.SaveFile(short_id, filename, data);

        if (fileInfo == null)
            return base.Problem("Something went wrong with saving the file. (Don't) contact the Administor for further information.");

        if (await _contextFileInfoRepository.SaveFile(fileInfo) == 0)
            return base.Problem("Something went wrong with saving the fileinfo. (Don't) contact the Administor for further information.");

        return Accepted();
    }

    [HttpDelete("delete")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<string>> DeleteFile()
    {
        _logger.LogInformation("Entering MainController.DeleteFile()");

        if (await CheckBlackList(Request))
            return Unauthorized();

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
    public async Task<ActionResult<string>> PostChat([FromBody] AiRequestDTO request)
    {
        _logger.Log(LogLevel.Debug, "Entered MainController.Chat");

        if (await CheckBlackList(Request))
            return Unauthorized();

        await _promptCounterService.AddPrompt();
        await _counterHub.Clients.All.RequestCounterChanged(
          new ISubscribeCounterClient.RequestsLeftParams()
          {
              PromptsLeftDay = _promptCounterService.PromptsLeftDay,
              PromptsLeftMinute = _promptCounterService.PromptsLeftMinute,
          });

        string short_id = GetShortUid(Request);

        List<KeyValuePair<string, string>> files = new List<KeyValuePair<string, string>>();
        foreach (var fileName in request.FileNames)
        {
            string[] content = await _fileHandling.LoadFile(short_id, fileName);
            files.Add(new(fileName, string.Join("\n", content)));
        }

        string response = await _geminiService.SendPrompt(request.Prompt, files);
        return Ok(response);
    }

    //[HttpPost("analyze")]
    //public async Task<ActionResult<string>> PostAnalyze()
    //{
    //    string response = await _geminiService.InitializeAi();
    //    return Ok(new { response });
    //}

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

    private async Task<bool> CheckBlackList(HttpRequest request)
    {
        var token = request.Headers.Authorization.ToString().Split(" ")[1];
        //Ich hoffe ich hab mit der manuellen Blacklist meine nicht meine Zeit verschwendet
        var blacklist = await _blackListRepository.GetAllTokensAsync();
        var reducedlist = blacklist.Select(b => b.Token);

        foreach (var listitem in reducedlist)
            if (listitem.Equals(token))
                return true;

        return false;
    }
}

