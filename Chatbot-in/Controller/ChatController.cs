using Chatbot_in.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot_in.Controller
{
    [ApiController]
    [Route("/api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public ChatController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet]
        [Route("filecontent")]
        public IActionResult GetFileContent([FromQuery] string filename)
        {
            var path = Path.Combine("YourFileStorageDirectory", filename);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var content = System.IO.File.ReadAllText(path);
            return Ok(content);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChatWithFile([FromBody] ChatRequest chatRequest)
        {
            if (string.IsNullOrEmpty(chatRequest.UserMessage) || string.IsNullOrEmpty(chatRequest.DocumentContent))
                return BadRequest("Missing user message or document content");

            // Send message + file content to Gemini
            var combinedPrompt = $"User question: {chatRequest.UserMessage}\n\nFile content:\n{chatRequest.DocumentContent}\n\nPlease summarize or answer based on the file.";

            var aiResponse = await _geminiService.ChatWithAI(combinedPrompt);

            return Ok(aiResponse);
        }
    }

}

public class ChatRequest
    {
        public string UserMessage { get; set; }
        public string DocumentContent { get; set; }
    }


