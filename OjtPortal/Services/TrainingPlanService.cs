using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;
using System.Text;

namespace OjtPortal.Services
{
    public interface ITrainingPlanService
    {
        Task<(TrainingPlan?, ErrorResponseModel?)> AddTrainingPlanAsync(NewTrainingPlanDto newTrainingPlan);
        Task<(List<TrainingPlanDto>?, ErrorResponseModel?)> GetSystemGeneratedTrainingPlansAsync();
        Task<(TrainingPlan?, ErrorResponseModel?)> GenerateSyntheticTrainingPlanAsync(TrainingPlanRequestDto requestDto);
        Task<(TrainingPlan?, ErrorResponseModel?)> GetTrainingPlanByIdAsync(int id);
        Task<(List<TrainingPlan>?, ErrorResponseModel?)> GetTrainingPlansByMentorAsync(int mentorId);
    }

    public class TrainingPlanService : ITrainingPlanService
    {
        private readonly ITrainingPlanRepo _trainingPlanRepo;
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        private readonly ILogger<TrainingPlanService> _logger;
        private readonly IStudentRepo _studentRepo;
        private readonly ICacheService _cache;

        public TrainingPlanService(ITrainingPlanRepo trainingPlanRepo, IMapper mapper, HttpClient client, ILogger<TrainingPlanService> logger, IStudentRepo studentRepo, ICacheService cache)
        {
            this._trainingPlanRepo = trainingPlanRepo;
            this._mapper = mapper;
            this._client = client;
            this._logger = logger;
            this._studentRepo = studentRepo;
            this._cache = cache;
        }

        public async Task<(TrainingPlan?, ErrorResponseModel?)> AddTrainingPlanAsync(NewTrainingPlanDto newTrainingPlan)
        {
            var trainingPlan = _mapper.Map<TrainingPlan>(newTrainingPlan);
            if (trainingPlan.MentorId == null)
            {
                trainingPlan.IsSystemGenerated = true;
                trainingPlan.Tasks.ForEach(t => t.IsSystemGenerated = true);
                trainingPlan.Tasks.ForEach(t => t.Skills.ForEach(s => s.IsSystemGenerated = true));
                trainingPlan.Tasks.ForEach(t => t.TechStacks.ForEach(ts => ts.IsSystemGenerated = true));
            }
            foreach(var task in trainingPlan.Tasks)
            {
                if(task.Difficulty == TaskDifficulty.Easy) trainingPlan.EasyTasksCount++;
                if (task.Difficulty == TaskDifficulty.Medium) trainingPlan.MediumTasksCount++;
                if (task.Difficulty == TaskDifficulty.Hard) trainingPlan.HardTasksCount++;
            }
            trainingPlan.TotalTasks = trainingPlan.Tasks.Count;
            trainingPlan = await _trainingPlanRepo.AddTrainingPlanAsync(trainingPlan);
            return (trainingPlan, null);
        }

        private async Task<(List<TrainingPlanDto>?, ErrorResponseModel?)> FetchAllTrainingPlansFromAPIAsync()
        {
            int maximumAttempts = 5;
            var errorMessage = string.Empty;
            for (int i = 1; i <= maximumAttempts; i++)
            {
                try
                {
                    var response = await _client.GetAsync("https://training-plan-microservice.graybeach-5685bf34.australiaeast.azurecontainerapps.io/training/plan");
                    if(!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to fetch training plans. Status Code: {response.StatusCode}, Message: {JObject.Parse(await response.Content.ReadAsStringAsync())["error"]}");
                    }
                    var readAsString = await response.Content.ReadAsStringAsync();
                    var trainingPlans = JsonConvert.DeserializeObject<TrainingPlanApiResponse>(readAsString);
                    var trainingPlansList = new List<TrainingPlan>();
                    foreach(var trainingPlan in trainingPlans!.TrainingPlans)
                    {
                        var trainingPlanEntity = _mapper.Map<TrainingPlan>(trainingPlan);
                        trainingPlanEntity.IsSystemGenerated = true;
                        trainingPlanEntity.Tasks.ForEach(t => t.IsSystemGenerated = true);
                        trainingPlanEntity.Tasks.ForEach(t => t.Skills.ForEach(s => s.IsSystemGenerated = true));
                        trainingPlanEntity.Tasks.ForEach(t => t.TechStacks.ForEach(ts => ts.IsSystemGenerated = true));
                        var addedTrainingPlan = await _trainingPlanRepo.AddTrainingPlanAsync(trainingPlanEntity);
                        if (addedTrainingPlan != null) trainingPlanEntity = addedTrainingPlan;
                        trainingPlansList.Add(trainingPlanEntity);
                    }
                    return (_mapper.Map<List<TrainingPlanDto>>(trainingPlansList), null);
                }
                catch (Exception ex)
                {
                    if (i == maximumAttempts) errorMessage = ex.Message;
                    _logger.LogError(ex.Message);
                }
            }
            return (null, new(HttpStatusCode.RequestTimeout, "Fetch on training plans api failed", errorMessage));
        }

        public async Task<(List<TrainingPlanDto>?, ErrorResponseModel?)> GetSystemGeneratedTrainingPlansAsync()
        {
            var trainingPlansFromCache = _cache.GetFromCache<List<TrainingPlan>>("trainingPlanList", "");
            if (trainingPlansFromCache != null) return (_mapper.Map<List<TrainingPlanDto>>(trainingPlansFromCache), null);
            var students = await _studentRepo.GetStudentsForTrainingPlanAsync();
            if (students != null)
            {
                var requestList = new List<TrainingPlanRequestDto>();
                foreach(var student in students)
                {
                    var request = _mapper.Map<TrainingPlanRequestDto>(student);
                    if (student.Shift == null || string.IsNullOrEmpty(request.Designation) || string.IsNullOrEmpty(request.Division)) continue;
                    request.DailyDutyHrs = student.Shift.DailyDutyHrs;
                    if(!requestList.Contains(request)) requestList.Add(request);
                }
                if(requestList != null)
                {
                    var trainingPlanList = new List<TrainingPlan>();
                    foreach(var request in requestList)
                    {
                        var trainingPlan = (await _trainingPlanRepo.CheckSystemGeneratedTrainingPlanAsync(request.Designation, request.Division, request.HrsToRender, request.DailyDutyHrs));
                        if(trainingPlan != null)
                        {
                            if (!trainingPlanList.Contains(trainingPlan)) trainingPlanList.Add(trainingPlan);
                            continue;
                        }
                        var (response, error) = await GenerateSyntheticTrainingPlanAsync(request);
                        if (error != null) return (null, error);
                        trainingPlan = response;
                        if (trainingPlan == null) continue;
                        trainingPlan = await _trainingPlanRepo.AddTrainingPlanAsync(trainingPlan);
                        if(!trainingPlanList.Contains(trainingPlan)) trainingPlanList.Add(trainingPlan);
                    }
                    _cache.AddToCache("trainingPlanList", "", trainingPlanList);
                    return (_mapper.Map<List<TrainingPlanDto>>(trainingPlanList), null);
                }
            }
            return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("student", "")));
        }

        public async Task<(TrainingPlan?, ErrorResponseModel?)> GenerateSyntheticTrainingPlanAsync(TrainingPlanRequestDto requestDto)
        {
            var existingTrainingPlan = (await _trainingPlanRepo.CheckSystemGeneratedTrainingPlanAsync(requestDto.Designation, requestDto.Division, requestDto.HrsToRender, requestDto.DailyDutyHrs));
            if (existingTrainingPlan != null)
            {
                return (existingTrainingPlan, null);
            }
            int maximumAttempts = 8;
            if (string.IsNullOrEmpty(requestDto.Designation) || string.IsNullOrEmpty(requestDto.Division) || requestDto.HrsToRender <= 0 || requestDto.DailyDutyHrs <= 0) return(null, null);
            var errorMessage = string.Empty;
            for (int i = 1; i <= maximumAttempts; i++)
            {
                _logger.LogInformation("Executing post GenerateSyntheticTrainingPlanAsync in attempt " + i);
                requestDto.Division = requestDto.Division.Replace("/", " or ");
                requestDto.Division = requestDto.Division.Replace("&", " and ");

                requestDto.Designation = requestDto.Designation.Replace("/", " or ");
                requestDto.Designation = requestDto.Designation.Replace("&", " and ");

                try
                {
                    var serializedRequest = JsonConvert.SerializeObject(requestDto);
                    _logger.LogInformation(serializedRequest);
                    var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

                    var response = await _client.PostAsync("https://training-plan-microservice.graybeach-5685bf34.australiaeast.azurecontainerapps.io/training/plan", content);
                    var readAsString = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode) throw new Exception($"Failed to fetch training plans. Status Code: {response.StatusCode}");
                    var trainingPlan = JsonConvert.DeserializeObject<TrainingPlanFromApiDto>(readAsString);
                    var trainingPlanEntity = _mapper.Map<TrainingPlan>(trainingPlan);
                    trainingPlanEntity.IsSystemGenerated = true;
                    trainingPlanEntity.Tasks.ForEach(t => t.IsSystemGenerated = true);
                    trainingPlanEntity.Tasks.ForEach(t => t.Skills.ForEach(s => s.IsSystemGenerated = true));
                    trainingPlanEntity.Tasks.ForEach(t => t.TechStacks.ForEach(ts => ts.IsSystemGenerated = true));
                    var addedTrainingPlan = await _trainingPlanRepo.AddTrainingPlanAsync(trainingPlanEntity);
                    if (addedTrainingPlan != null) trainingPlanEntity = addedTrainingPlan;
                    return (trainingPlanEntity, null);
                }
                catch (Exception ex)
                {
                    if (i == maximumAttempts) errorMessage = ex.Message;
                    _logger.LogError($"Attempt {i} failed.");
                }
            }
            return (new(), new(HttpStatusCode.RequestTimeout, "Fetch on external api failed", errorMessage));
        }

        public async Task<(TrainingPlan?, ErrorResponseModel?)> GetTrainingPlanByIdAsync(int id)
        {
            var trainingPlan = await _trainingPlanRepo.GetTrainingPlanByIdAsync(id);
            if (trainingPlan == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("training plan"), LoggingTemplate.MissingRecordDescription("training plan", id.ToString())));
            return (trainingPlan, null);
        }

        public async Task<(List<TrainingPlan>?, ErrorResponseModel?)> GetTrainingPlansByMentorAsync(int mentorId)
        {
            var trainingPlan = await _trainingPlanRepo.GetTrainingPlansByMentorAsync(mentorId);
            if (trainingPlan == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("training plan"), LoggingTemplate.MissingRecordDescription("training plan", mentorId.ToString())));
            return (trainingPlan, null);
        }

    }

}
