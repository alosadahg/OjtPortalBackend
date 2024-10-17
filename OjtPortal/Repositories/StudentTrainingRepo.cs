using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;
using System.Runtime.CompilerServices;

namespace OjtPortal.Repositories
{
    public interface IStudentTrainingRepo
    {
        Task<StudentTraining> AddStudentTrainingAsync(StudentTraining studentTraining);
        Task<StudentTraining?> GetStudentTrainingAsync(int studentId);
    }

    public class StudentTrainingRepo : IStudentTrainingRepo
    {
        private readonly OjtPortalContext _context;
        private readonly ILogger<StudentTrainingRepo> _logger;

        public StudentTrainingRepo(OjtPortalContext context, ILogger<StudentTrainingRepo> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task<StudentTraining> AddStudentTrainingAsync(StudentTraining studentTraining)
        {
            try
            {
                var existing = await _context.StudentTrainings.Include(st=>st.Tasks).FirstOrDefaultAsync(st => st.StudentId==studentTraining.StudentId);
                if (existing != null) return existing;
                await _context.StudentTrainings.AddAsync(studentTraining);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
            return studentTraining;
        }

        public async Task<StudentTraining?> GetStudentTrainingAsync(int studentId)
        {
            return await _context.StudentTrainings.Include(st => st.TrainingPlan).Include(st => st.Tasks).ThenInclude(t => t.TrainingTask).ThenInclude(t => t.TechStacks).Include(st => st.Tasks).ThenInclude(t => t.TrainingTask).ThenInclude(t => t.Skills).FirstOrDefaultAsync(st => st.StudentId == studentId);
        }
    }
}
