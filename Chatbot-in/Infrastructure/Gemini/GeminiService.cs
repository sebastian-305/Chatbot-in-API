
using System.Text.Json;
using System.Text;

namespace Chatbot_in.Domain.Service
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["Gemini:Key"];
        }

        public async Task<string> AnalyzeText(string text)
        {
            var requestBody = new
            {
                model = "gemini-1.5-flash",
                prompt = new
                {
                    text = $"Analyze this document:\n{text}"
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateText?key={_apiKey}", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
