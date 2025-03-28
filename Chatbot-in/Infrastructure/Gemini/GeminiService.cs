
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
            _model = "gemini-1.5-flash-001";
            _logger = logger;
        }


        //Die Mindestanzahl der Eingabetokens für das Kontext-Caching beträgt 32.768 -- Für unsren Use-Case overkill
        public async Task<string> InitializeAi(string short_uid)
        {
            var requestUri = $"https://generativelanguage.googleapis.com/v1beta/cachedContents?key={_apiKey}";

            try
            {
                var fileContent = Convert.ToBase64String(File.ReadAllBytes("a11.txt"));

                var context = new
                {
                    model = "models/gemini-1.5-flash-001",
                    contents = new[]
                    {
                new
                {
                    parts = new object[]
                    {
                        new
                        {
                            inline_data = new
                            {
                                mime_type = "text/plain",
                                data = fileContent
                            }
                        }
                    },
                    role = "user"
                }
            },
                    systemInstruction = new
                    {
                        parts = new object[]
                        {
                    new
                    {
                        text = "You are an expert at analyzing transcripts."
                    }
                        }
                    },
                    ttl = "300s"
                    // Fügen Sie hier ggf. weitere optionale Felder mit Standardwerten hinzu, z.B. safetySettings
                };

                var jsonContent = JsonSerializer.Serialize(context);
                //_logger.LogDebug(jsonContent);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(requestUri, content);
                var responseString = await response.Content.ReadAsStringAsync();
                _logger.LogDebug(responseString);
                response.EnsureSuccessStatusCode();
                return responseString ?? "";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request Error: {ex.Message}, Status Code: {ex.StatusCode}");
                
                return "Something went wrong with the API request.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return "Something went wrong";
            }
        }


        //Deprecated
        public async Task<string> AnalyzeText(string prompt)
        {
            var requestBody = new
            {
                model = _model,
                prompt = new
                {
                    text = $"Analyze this document:\n{prompt}"
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={_apiKey}", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }


        public async Task<string> ChatWithFiles(string prompt, List<KeyValuePair<string,string>> files)
        {
            StringBuilder promptBuilder = new StringBuilder();

            promptBuilder.Append("You are an expert in analyzing Texts and answering Questions. I have the following question that might be in another language:\n\n");
            promptBuilder.Append(prompt);
            promptBuilder.Append("\n\nThis was my Question. Please include your reasoning in the answer and answer it considering ");
            promptBuilder.Append((files.Count > 1 ? "these files" : "this file"));
            promptBuilder.Append(" that might be in another language");
            promptBuilder.Append("---------------------------------");
            foreach (var file in files)
            {                
                promptBuilder.Append("\n\nFilename: ");
                promptBuilder.Append(file.Key);
                promptBuilder.Append("\n\nFilecontent: \n");
                promptBuilder.Append(file.Value);
                promptBuilder.Append("---------------------------------");
            }
            promptBuilder.Append("\n\nI gave you now the question and all the files in textform for the answer. Please answer the question in the language the question is written.");

            _logger.LogInformation(promptBuilder.ToString());
            return await ChatWithAI(promptBuilder.ToString());
        }

        public async Task<string> ChatWithAI(string userMessage)
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
                            text = userMessage
                        }
                    },
            //        role = "user"
                }, 
                
            },
          //      cachedContent = "cachedContents/fsowom1ynsmg",
            };



            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);


                var responseString = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<GeminiReponse>(responseString);

                _logger.LogDebug(responseString);

                response.EnsureSuccessStatusCode();

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
