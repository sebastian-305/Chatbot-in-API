using Chatbot_in.Domain.Service;
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

        [HttpPost]
        public async Task<IActionResult> ChatWithFile([FromBody] ChatRequest chatRequest)
        {
            if (string.IsNullOrEmpty(chatRequest.UserMessage) || string.IsNullOrEmpty(chatRequest.DocumentContent))
                return BadRequest("Missing user message or document content");

            string response = await _geminiService.ChatWithAI(chatRequest.UserMessage, chatRequest.DocumentContent);
            return Ok(new { response });
        }
    }

    public class ChatRequest
    {
        public string UserMessage { get; set; }
        public string DocumentContent { get; set; }
    }
}
}
