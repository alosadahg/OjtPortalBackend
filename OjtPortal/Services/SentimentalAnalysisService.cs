using OjtPortal.Dtos;
using OjtPortal.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace OjtPortal.Services
{
    public interface ISentimentalAnalysisService
    {
        Task<(SentimentAnalysisDto?, ErrorResponseModel?)> AnalyzeSentimentAsync(string inputText);
    }

    public class SentimentalAnalysisService : ISentimentalAnalysisService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SentimentalAnalysisService> _logger;
        private readonly string _apiUrl = "https://api-inference.huggingface.co/models/cardiffnlp/twitter-roberta-base-sentiment-latest";
        private string _apiKey = "";

        public SentimentalAnalysisService(HttpClient httpClient, IConfiguration configuration, ILogger<SentimentalAnalysisService> logger)
        {
            _httpClient = httpClient;
            this._configuration = configuration;
            this._logger = logger;
        }

        public async Task<(SentimentAnalysisDto?, ErrorResponseModel?)> AnalyzeSentimentAsync(string inputText)
        {
            inputText = inputText.Replace("/", " or ");
            inputText = inputText.Replace("&", " and ");

            var payload = new
            {
                inputs = inputText
            };

            try
            {
                var response = await QueryAsync(payload);
                if (response != null)
                {
                    var label = response[0][0].GetProperty("label").GetString();
                    var score = response[0][0].GetProperty("score").GetSingle();

                    var analysis = new SentimentAnalysisDto()
                    {
                        Label = label,
                        Score = score
                    };
                    return (analysis, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Sentiment analysis error: " + ex.Message);
                return (null, new(HttpStatusCode.RequestTimeout, "Failed to get sentiment analysis", "Encountered a request timeout. Please try again."));
            }

            return (null, new(HttpStatusCode.BadRequest, "Failed to get analysis", "Please try again."));
        }

        private async Task<JsonElement[][]?> QueryAsync(object payload)
        {
            _apiKey = _configuration["HUGGINGFACE_APIKEY"];
            var maximumAttempts = 10;
            for (int i = 1; i <= maximumAttempts; i++)
            {
                _logger.LogInformation($"Trying attempt {i} to get sentiment analysis");
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                    var response = await _httpClient.PostAsJsonAsync(_apiUrl, payload);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement[][]>(responseString);

                    return jsonResponse!;
                }
                catch (Exception)
                {
                    _logger.LogInformation($"Attempt {i} failed. Retrying to get the sentiment analysis.");
                }
            }
            return null;
        }
    }
}
