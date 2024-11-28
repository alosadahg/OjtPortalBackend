using System.Net;
using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ISubMentorService
    {
        Task<(SubMentorDto?, ErrorResponseModel?)> RegisterSubmentor(int mentorId, int submentorId);
        Task<(FullMentorDtoWithStudents?, ErrorResponseModel?)> TransferMentorshipToSubmentorAsync(int previousMentorId, int subMentorId);
        Task<(FullMentorDtoWithSubMentors?, ErrorResponseModel?)> GetSubmentorsByHeadMentorIdAsync(int headMentorId);
        Task<(CompanyWithFullMentorsDto?, ErrorResponseModel?)> GetMentorsWithNoHeadMentorsAsync(int companyId);
        Task<(SubMentorWithTasksDto?, ErrorResponseModel?)> DelegateSubmentorToTaskAsync(int mentorId, int submentorId, int taskId);
    }

    public class SubMentorService : ISubMentorService
    {
        private readonly IMentorRepo _mentorRepo;
        private readonly ISubMentorRepo _subMentorRepo;
        private readonly IMapper _mapper;
        private readonly ITrainingPlanRepo _trainingPlanRepo;
        private readonly ICompanyRepo _companyRepo;
        private readonly ITaskRepo _taskRepo;

        public SubMentorService(IMentorRepo mentorRepo, ISubMentorRepo subMentorRepo, IMapper mapper, ITrainingPlanRepo trainingPlanRepo, ICompanyRepo companyRepo, ITaskRepo taskRepo)
        {
            this._mentorRepo = mentorRepo;
            this._subMentorRepo = subMentorRepo;
            this._mapper = mapper;
            this._trainingPlanRepo = trainingPlanRepo;
            this._companyRepo = companyRepo;
            this._taskRepo = taskRepo;
        }
        public async Task<(SubMentorDto?, ErrorResponseModel?)> RegisterSubmentor(int mentorId, int submentorId)
        {
            var existingMentor = await _mentorRepo.GetMentorByIdAsync(mentorId, false, false, true);
            if (existingMentor == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("mentor"), LoggingTemplate.MissingRecordDescription("mentor", mentorId.ToString())));
            if (mentorId == submentorId) return (null, new ErrorResponseModel(HttpStatusCode.UnprocessableEntity, "Invalid ID", "Cannot delegate submentor to itself"));
            var existingSubmentor = await _mentorRepo.GetMentorByIdAsync(submentorId, false, false, true);
            if (existingSubmentor == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("submentor"), LoggingTemplate.MissingRecordDescription("submentor", submentorId.ToString())));

            if (existingMentor.CompanyId != existingSubmentor.CompanyId)
                return (null, new ErrorResponseModel(HttpStatusCode.MethodNotAllowed, "Invalid Company", "Submentor and mentor have different companies"));

            var isExisting = await _subMentorRepo.IsRecordExisting(mentorId, submentorId);
            if (isExisting != null && isExisting.HeadMentorId == submentorId) return (null, new ErrorResponseModel(HttpStatusCode.MethodNotAllowed, "Invalid IDs", "This submentor ID is the head mentor"));

            var newSubmentor = new SubMentor
            {
                SubmentorId = existingSubmentor.UserId,
                Submentor = existingSubmentor,
                HeadMentor = existingMentor
            };
            newSubmentor = await _subMentorRepo.AddSubMentorAsync(newSubmentor);
            if (newSubmentor == null) return (null, new ErrorResponseModel(HttpStatusCode.UnprocessableContent, "Submentor creation failed", "Please check logs"));
            return (_mapper.Map<SubMentorDto>(newSubmentor), null);
        }

        public async Task<(FullMentorDtoWithStudents?, ErrorResponseModel?)> TransferMentorshipToSubmentorAsync(int previousMentorId, int subMentorId)
        {
            var previousExisting = await _mentorRepo.GetMentorByIdAsync(previousMentorId, true, false, true);
            if (previousExisting == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("mentor"), LoggingTemplate.MissingRecordDescription("mentor", previousMentorId.ToString())));
            if (previousExisting.SubMentors != null)
            {
                var submentor = previousExisting.SubMentors.FirstOrDefault(sb => sb.SubmentorId == subMentorId);
                if (submentor != null)
                {
                    var students = previousExisting.Students!.ToList();
                    var trainingPlans = await _trainingPlanRepo.GetTrainingPlansByMentorAsync(previousExisting.UserId);
                    var submentors = previousExisting.SubMentors.ToList();
                    submentors.Remove(submentor);
                    var updatedSubmentor = await _subMentorRepo.TransferMentorshipToSubmentorAsync(trainingPlans, students, submentors, submentor);
                    return (_mapper.Map<FullMentorDtoWithStudents>(updatedSubmentor), null);
                }
            }
            return (null, new ErrorResponseModel(HttpStatusCode.NotFound, "Missing submentor", "Submentor not found reporting under mentor"));
        }

        public async Task<(FullMentorDtoWithSubMentors?, ErrorResponseModel?)> GetSubmentorsByHeadMentorIdAsync(int headMentorId)
        {
            var existing = await _mentorRepo.GetMentorByIdAsync(headMentorId, false, false, true);
            if (existing == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("head mentor"), LoggingTemplate.MissingRecordDescription("head mentor", headMentorId.ToString())));
            var mapped = _mapper.Map<FullMentorDtoWithSubMentors>(existing);
            if (existing.SubMentors != null)
            {
                var subMentorList = new List<Mentor>();
                existing.SubMentors.ToList().ForEach(sb =>
                {
                    subMentorList.Add(sb.Submentor!);
                });
                mapped.SubMentors = _mapper.Map<List<MentorDto>>(subMentorList);
            }
            return (mapped, null);
        }

        public async Task<(CompanyWithFullMentorsDto?, ErrorResponseModel?)> GetMentorsWithNoHeadMentorsAsync(int companyId)
        {
            var existingCompany = await _companyRepo.GetCompanyWithMentorsFullAsync(companyId);
            if (existingCompany == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("company"), LoggingTemplate.MissingRecordDescription("company", companyId.ToString())));
            if(existingCompany.Mentors != null)
            {
                var mentorsCopy = existingCompany.Mentors.ToList();
                foreach (var mentor in existingCompany.Mentors)
                {
                    var headmentorExists = await _subMentorRepo.HasHeadMentorAsync(mentor.UserId);
                    if (headmentorExists) mentorsCopy.Remove(mentor);
                }
                existingCompany.Mentors = mentorsCopy;
            }
            return (_mapper.Map<CompanyWithFullMentorsDto>(existingCompany), null);
        }

        public async Task<(SubMentorWithTasksDto?, ErrorResponseModel?)> DelegateSubmentorToTaskAsync(int mentorId, int submentorId, int taskId)
        {
            var existingMentor = await _mentorRepo.GetMentorByIdAsync(mentorId, false, false, true);
            if (existingMentor == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("head mentor"), LoggingTemplate.MissingRecordDescription("head mentor", mentorId.ToString())));
            var subMentors = existingMentor.SubMentors;
            var existingTask = await _taskRepo.GetTaskByIdAsync(taskId);
            if (existingTask == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("task"), LoggingTemplate.MissingRecordDescription("task", taskId.ToString())));
            var trainingPlan = existingTask.TrainingPlan;
            if (trainingPlan!.MentorId != mentorId) return (null, new ErrorResponseModel(HttpStatusCode.MethodNotAllowed, "Unauthorized mentor", "Access not allowed"));
            if (subMentors != null)
            {
                foreach (var submentor in subMentors)
                {
                    var sub = await _subMentorRepo.GetSubMentorByIdAsync(submentor.SubmentorId, true);
                    if (sub!.SubmentorId == submentorId)
                    {
                        sub = await _subMentorRepo.AssignTaskToSubmentorAsync(sub, existingTask);
                        return (_mapper.Map<SubMentorWithTasksDto>(sub), null);
                    }
                }
            }
            return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("submentor"), LoggingTemplate.MissingRecordDescription("submentor", submentorId.ToString())));
        }
    }
}
