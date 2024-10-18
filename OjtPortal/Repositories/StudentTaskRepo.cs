using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;
using OjtPortal.Enums;

namespace OjtPortal.Repositories
{
    public interface IStudentTaskRepo
    {
        Task<StudentTask?> GetStudentTaskByIdAsync(int studentId, int taskId);
        Task<StudentTask?> UpdateStudentTaskStatusAsync(StudentTask task, TrainingTaskStatus updatedStatus);
    }

    public class StudentTaskRepo : IStudentTaskRepo
    {
        private readonly OjtPortalContext _context;

        public StudentTaskRepo(OjtPortalContext context)
        {
            this._context = context;
        }
        public async Task<StudentTask?> GetStudentTaskByIdAsync(int studentId, int taskId)
        {
            return await _context.StudentTasks.Include(st => st.StudentTraining).ThenInclude(st => st.TrainingPlan).Include(st => st.TrainingTask).ThenInclude(t => t.Skills).Include(st => st.TrainingTask).ThenInclude(t => t.TechStacks).FirstOrDefaultAsync(st => st.TrainingTaskId == taskId && st.StudentTrainingId == studentId);
        }

        public async Task<StudentTask> UpdateStudentTaskStatusAsync(StudentTask task, TrainingTaskStatus updatedStatus)
        {
            try
            {
                task.TaskStatus = updatedStatus;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
            return task;
        }
    }
}
