using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IStudentTaskService
    {
        Task<(StudentTaskDto?, ErrorResponseModel?)> UpdateTaskStatus(int studentId, int taskId, TrainingTaskStatus updatedStatus);
    }

    public class StudentTaskService : IStudentTaskService
    {
        private readonly IStudentTaskRepo _studentTaskRepo;
        private readonly IMapper _mapper;

        public StudentTaskService(IStudentTaskRepo studentTaskRepo, IMapper mapper)
        {
            this._studentTaskRepo = studentTaskRepo;
            this._mapper = mapper;
        }

        public async Task<(StudentTaskDto?, ErrorResponseModel?)> UpdateTaskStatus(int studentId, int taskId, TrainingTaskStatus updatedStatus)
        {
            var currentDate= DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")));
            var studentTask = await _studentTaskRepo.GetStudentTaskByIdAsync(studentId, taskId);
            if (studentTask == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student task"), LoggingTemplate.MissingRecordDescription("student task", taskId.ToString())));

            if(!(studentTask.TaskStatus == TrainingTaskStatus.Done || studentTask.TaskStatus == TrainingTaskStatus.DoneLate))
            {
                if (updatedStatus == TrainingTaskStatus.Done)
                {
                    if (studentTask.DueDate != null && currentDate > studentTask.DueDate) updatedStatus = TrainingTaskStatus.DoneLate;
                    studentTask.DateCompleted = currentDate;
                    studentTask.StudentTraining!.CompletedTaskCount++;
                    studentTask.StudentTraining.TrainingPlanStatus = TrainingTaskStatus.InProgress;
                    if (studentTask.StudentTraining.CompletedTaskCount >= studentTask.StudentTraining.TrainingPlan.TotalTasks)
                    {
                        studentTask.StudentTraining.TrainingPlanStatus = TrainingTaskStatus.Done;
                    }
                }

                var updated = await _studentTaskRepo.UpdateStudentTaskStatusAsync(studentTask, updatedStatus);
                if (updated == null) return (null, new(HttpStatusCode.BadRequest, "Update failed", "Please check logs"));
            }
            return (_mapper.Map<StudentTaskDto>(studentTask), null);
        }
    }
}
