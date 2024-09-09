using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IHolidayService
    {
        Task<(List<Holiday>?, ErrorResponseModel?)> GetHolidaysAsync();
    }

    public class HolidayService : IHolidayService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;
        private readonly IHolidayRepository _holidayRepository;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;
        private static List<Holiday> _holidays = new();

        public HolidayService(ILogger<HolidayService> logger, HttpClient client, IHolidayRepository holidayRepository, ICacheService cacheService, IConfiguration configuration)
        {
            this._logger = logger;
            this._client = client;
            this._holidayRepository = holidayRepository;
            this._cacheService = cacheService;
            this._configuration = configuration;
        }

        public async Task<(List<Holiday>?, ErrorResponseModel?)> GetHolidaysAsync()
        {
            if(_holidays.Any())
            {
                _logger.LogInformation("Returning existing local holidays.");
                return (_holidays, null);
            }
            var cacheKey = "holidays";
            var existingHolidays = _cacheService.GetFromPermanentCache<List<Holiday>>(cacheKey);
            var yearToCheck = DateTime.UtcNow.Year;
            if (existingHolidays == null)
            {
                existingHolidays = await _holidayRepository.GetHolidaysAsync(yearToCheck);
                if(existingHolidays!.Any()) _cacheService.AddToPermanentCache(cacheKey, existingHolidays!);
            }
            
            if (existingHolidays!.Any())
            {
                if (existingHolidays![0].Date.Year != yearToCheck)
                {
                    _logger.LogInformation($"Outdated holidays. Updating from third-party API...");
                    _cacheService.RemoveFromPermanentCache(cacheKey);
                    return await GetHolidaysFromAPIAsync(cacheKey, yearToCheck);
                } 
                else
                {
                    _holidays = existingHolidays;
                    return (existingHolidays, null);
                }
            }
            _logger.LogInformation($"Database miss for {cacheKey}. Fetching from third-party API...");
            return await GetHolidaysFromAPIAsync(cacheKey,yearToCheck);
        }

        private async Task<(List<Holiday>?, ErrorResponseModel?)> GetHolidaysFromAPIAsync(string cacheKey, int yearToCheck)
        {
            var calendarificApiKey = _configuration["CalendarificApiKey"];
            var url = $"https://calendarific.com/api/v2/holidays?&api_key={calendarificApiKey}&country=PH&year={yearToCheck}";

            HttpResponseMessage response = await _client.GetAsync(url);

            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch holidays. Status code: {response.StatusCode}, Message: {JObject.Parse(await response.Content.ReadAsStringAsync())["error"]}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.StackTrace}");
                _logger.LogError($"{ex.Message}");
                return (null, new ErrorResponseModel(HttpStatusCode.BadRequest, "Error in Fetching Holidays", "Failed to fetch holidays. Check logs for details."));
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString)["response"]!;

            List<Holiday> result = new();

            foreach (var holidayJson in json["holidays"]!)
            {
                var type = (string)holidayJson["primary_type"]!;
                if (String.Compare(type, "Regular Holiday", true) == 0 || String.Compare(type, "Special Non-working Holiday", true) == 0)
                {
                    var holiday = new Holiday
                    {
                        Name = (string)holidayJson["name"]!,
                        Description = (string)holidayJson["description"]!,
                        Date = (DateOnly.Parse((string)holidayJson["date"]!["iso"]!)),
                        Type = (string)holidayJson["primary_type"]!,
                        Country = (string)holidayJson["country"]!["name"]!
                    };
                    result.Add(holiday);
                }
            }
            await _holidayRepository.AddAllHolidaysAsync(result);
            _cacheService.AddToPermanentCache(cacheKey, result);
            _holidays = result;
            return (result, null);
        }
    }
}
