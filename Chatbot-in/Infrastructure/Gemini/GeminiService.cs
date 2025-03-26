
using System.Text.Json;
using System.Text;
using Chatbot_in.Infrastructure.Gemini;

namespace Chatbot_in.Domain.Service
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["Gemini:Key"] ?? "";
            _model = "gemini-1.5-flash";
            _logger = logger;
        }

        public async Task<string> AnalyzeText(string text)
        {
            var requestBody = new
            {
                model = _model,
                prompt = new
                {
                    text = $"Analyze this document:\n{text}"
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={_apiKey}", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }


        public async Task<string> ChatWithAI(string userMessage, string documentContent)
        {
            var requestUri = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";


            var requestBody = new
            {
                contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = "How many people live in Austria?"
                        }
                    }
                }
            }
            };



            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<GeminiReponse>(responseString);


                return result?.candidates[0]?.content?.parts[0]?.text ?? String.Empty; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "Error";
            }


        }
    }
}
