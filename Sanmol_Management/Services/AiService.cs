using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sanmol_Management.Services
{
    public class AiService
    {
        // Reuse HttpClient (IMPORTANT for performance)
        private static readonly HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private readonly string apiKey;

        public AiService()
        {
            apiKey = ConfigurationManager.AppSettings["GroqApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Groq API key not configured in Web.config");
            }

            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }
        }

        public async Task<string> AskAI(string userMessage, string dbContext)
        {
            try
            {
                string systemPrompt =
                    "You are an intelligent assistant for Sanmol Management System. " +
                    "Answer clearly and professionally using ONLY the provided database data. " +
                    "If the answer is not in the database data, say: 'That information is not available in the system.'";

                string userPrompt =
                    $"Database Data:\n{dbContext}\n\n" +
                    $"User Question:\n{userMessage}\n\n" +
                    $"Provide a short and clear answer.";

                var requestBody = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    },
                    temperature = 0.4,
                    max_tokens = 400
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    "https://api.groq.com/openai/v1/chat/completions",
                    content
                );

                var responseString = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine("Groq Status: " + response.StatusCode);
                System.Diagnostics.Debug.WriteLine("Groq Response: " + responseString);

                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var errorObj = JObject.Parse(responseString);
                        var errorMessage = errorObj["error"]?["message"]?.ToString()
                                           ?? "Unknown API error";

                        return $"⚠️ AI Service Error: {errorMessage}";
                    }
                    catch
                    {
                        return $"⚠️ AI Service Error (Status {response.StatusCode})";
                    }
                }

                var result = JObject.Parse(responseString);

                var messageContent =
                    result["choices"]?[0]?["message"]?["content"]?.ToString();

                if (!string.IsNullOrWhiteSpace(messageContent))
                {
                    return messageContent.Trim();
                }

                return "I couldn't generate a proper response. Please try again.";
            }
            catch (TaskCanceledException)
            {
                return "⏱️ AI request timed out. Please try again.";
            }
            catch (HttpRequestException ex)
            {
                return $"🌐 Network error: {ex.Message}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AI Exception: " + ex);
                return "❌ Unexpected error occurred while contacting AI service.";
            }
        }
    }
}
