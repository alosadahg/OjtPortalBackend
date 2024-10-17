using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IStudentTrainingService
    {
        Task<(StudentTraining?, ErrorResponseModel?)> AssignTrainingPlanAsync(AssignTrainingPlanDto assignTrainingPlanDto, int mentorId);
        Task<(AssignedTrainingPlanToStudentDto?, ErrorResponseModel?)> GetAssignedTrainingPlanToStudentAsync(int studentId);
    }

    public class StudentTrainingService : IStudentTrainingService
    {
        private readonly IStudentTrainingRepo _studentTrainingRepo;
        private readonly ITrainingPlanRepo _trainingPlanRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IMapper _mapper;

        public StudentTrainingService(IStudentTrainingRepo studentTrainingRepo, ITrainingPlanRepo trainingPlanRepo, IStudentRepo studentRepo, IMapper mapper)
        {
            this._studentTrainingRepo = studentTrainingRepo;
            this._trainingPlanRepo = trainingPlanRepo;
            this._studentRepo = studentRepo;
            this._mapper = mapper;
        }

        public async Task<(StudentTraining?, ErrorResponseModel?)> AssignTrainingPlanAsync(AssignTrainingPlanDto assignTrainingPlanDto, int mentorId)
        {
            var trainingPlans = await _trainingPlanRepo.GetTrainingPlansByMentorAsync(mentorId);
            if (!trainingPlans.Any()) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("training plans"), LoggingTemplate.MissingRecordDescription("training plans", mentorId.ToString())));
            var trainingPlan = trainingPlans.Where(tp => tp.Id == assignTrainingPlanDto.TrainingPlanId).FirstOrDefault();
            if (trainingPlan == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("training plan"), LoggingTemplate.MissingRecordDescription("training plan", assignTrainingPlanDto.TrainingPlanId.ToString())));
            var student = await _studentRepo.GetStudentByIdAsync(assignTrainingPlanDto.StudentId, true, false, false);
            if (student == null || student.MentorId != mentorId) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("training plans", assignTrainingPlanDto.StudentId.ToString())));
            var studentTraining = _mapper.Map<StudentTraining>(assignTrainingPlanDto);
            studentTraining.ExpectedEndDate = student.EndDate!.Value;
            studentTraining.ExpectedStartDate = student.StartDate!.Value;
            studentTraining.DurationInHours = student.HrsToRender;

            foreach (var task in trainingPlan.Tasks)
            {
                if (assignTrainingPlanDto.TaskWithDueDtos != null)
                {
                    var taskWithDue = assignTrainingPlanDto.TaskWithDueDtos!.Where(t => t.TaskId == task.Id).FirstOrDefault();
                    var studentTask = new StudentTask
                    {
                        Student = student,
                        TrainingTask = task,
                        DueDate = (taskWithDue != null && taskWithDue.DueDate != null) ? taskWithDue.DueDate : null
                    };
                    studentTraining.Tasks.Add(studentTask);
                }
            }

            var addedStudentTraining = await _studentTrainingRepo.AddStudentTrainingAsync(studentTraining);
            if (addedStudentTraining != null) studentTraining = addedStudentTraining;
            return (studentTraining, null);
        }

        public async Task<(AssignedTrainingPlanToStudentDto?, ErrorResponseModel?)> GetAssignedTrainingPlanToStudentAsync(int studentId)
        {
            var existing = await _studentTrainingRepo.GetStudentTrainingAsync(studentId);
            if (existing == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("training plan"), LoggingTemplate.MissingRecordDescription("training plan", studentId.ToString())));
            var mappedExisting = _mapper.Map<AssignedTrainingPlanToStudentDto>(existing);
            mappedExisting.Title = existing.TrainingPlan!.Title;
            mappedExisting.Description = existing.TrainingPlan.Description;
            return (mappedExisting, null);
        }
    }
}
