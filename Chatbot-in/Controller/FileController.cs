using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Repositories;
using Chatbot_in.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controller
{
    [ApiController]
    [Route("/api/files")]
    public class FileController : ControllerBase
    {

        private readonly IContextFileInfoRepository _fileInfoRepository;
        private readonly ILogger<FileController> _logger;
        private readonly string _uploadFolder;

        public FileController(IContextFileInfoRepository fileInfoRepository, ILogger<FileController> logger, IConfiguration config)
        {
            _fileInfoRepository = fileInfoRepository;
            _logger = logger;
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads"); // Optional: use config later
            Directory.CreateDirectory(_uploadFolder);
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not provided");

            var sessionToken = Request.Headers["Authorization"].ToString();

            // Save content to disk
            var savePath = Path.Combine(_uploadFolder, file.FileName);
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save metadata
            var fileInfo = new ContextFileInfo
            {
                FileName = file.FileName,
                OwnerToken = sessionToken
            };
            await _fileInfoRepository.SaveFile(fileInfo);

            _logger.LogInformation($"File uploaded: {file.FileName}");

            return Ok(new ResponseDTO { Status = "Success", Message = "File uploaded and saved." });
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFiles()
        {
            var sessionToken = Request.Headers["Authorization"].ToString();
            var files = await _fileInfoRepository.GetFiles(sessionToken);
            return Ok(files.Select(f => new { f.FileName }));
        }

        [HttpGet("filecontent")]
        [Authorize]
        public async Task<IActionResult> GetFileContent([FromQuery] string filename)
        {
            var sessionToken = Request.Headers["Authorization"].ToString();
            var files = await _fileInfoRepository.GetFiles(sessionToken);

            if (!files.Any(f => f.FileName == filename))
                return Unauthorized("You don't have access to this file.");

            var path = Path.Combine(_uploadFolder, filename);
            if (!System.IO.File.Exists(path))
                return NotFound("File not found.");

            var content = await System.IO.File.ReadAllTextAsync(path);
            return Ok(content);
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteFile([FromHeader] string filename)
        {
            var sessionToken = Request.Headers["Authorization"].ToString();
            var file = (await _fileInfoRepository.GetFiles(sessionToken)).FirstOrDefault(f => f.FileName == filename);

            if (file == null)
                return NotFound("File not found.");

            var path = Path.Combine(_uploadFolder, filename);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            await _fileInfoRepository.DeleteFile(file);
            return Ok(new ResponseDTO { Status = "Success", Message = "File deleted." });
        }
    }
}
    

