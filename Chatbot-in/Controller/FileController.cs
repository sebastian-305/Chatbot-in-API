using Chatbot_in.Domain.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controller
{
    [ApiController]
    [Route("/api/files")]
    public class FileController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public FileController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not provided");

            string fileContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            string analysisResult = await _geminiService.AnalyzeText(fileContent);
            return Ok(new { message = "File uploaded and analyzed", analysis = analysisResult });
        }
    }
}
