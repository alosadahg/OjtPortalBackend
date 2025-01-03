﻿using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ITeacherRepo
    {
        Task<Teacher?> AddTeacherAsync(Teacher newTeacher);
        Task<Teacher?> GetTeacherByIdAsync(int id,bool includeUser);
        Task<bool> IsTeacherExisting(Teacher instructor);
        Task<List<Teacher>?> GetTeacherByDepartmentAsync(Department department);
        Task<List<Teacher>?> GetTeachersAsync();
        Task<List<Teacher>?> GetTeacherByDepartmentIdAsync(int departmentId);
    }

    public class TeacherRepo : ITeacherRepo
    {
        private readonly OjtPortalContext _context;

        public TeacherRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<Teacher?> AddTeacherAsync(Teacher newTeacher)
        {
            if (IsTeacherExisting(newTeacher).Result) return null;
            _context.Entry(newTeacher.Department).State = EntityState.Unchanged;
            await _context.Teachers.AddAsync(newTeacher);
            await _context.SaveChangesAsync();
            return newTeacher;
        }

        public async Task<bool> IsTeacherExisting(Teacher instructor)
        {
            return await _context.Teachers.ContainsAsync(instructor);
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id, bool includeStudents)
        {
            IQueryable<Teacher> query = _context.Teachers.Include(t => t.User).Include(t => t.Department);
            if (includeStudents)
            {
                query = query.Include(m => m.Students)!.ThenInclude(s => s.DegreeProgram);
                query = query.Include(m => m.Students)!.ThenInclude(s => s.User);
                query = query = query.Include(m => m.Students)!.ThenInclude(s => s.Mentor).ThenInclude(m => m.User);
                query = query = query.Include(m => m.Students)!.ThenInclude(s => s.Mentor).ThenInclude(m => m.Company);
            }
            return await query.FirstOrDefaultAsync(m => m.UserId == id);
        }

        public async Task<List<Teacher>?> GetTeacherByDepartmentAsync(Department department)
        {
            return await _context.Teachers.Include(t => t.User).Include(t => t.Department).Include(t => t.Students).Where(t => t.Department == department).ToListAsync();
        }

        public async Task<List<Teacher>?> GetTeacherByDepartmentIdAsync(int departmentId)
        {
            return await _context.Teachers.Include(t => t.User).Include(t => t.Department).Include(t => t.Students).Where(t => t.Department.DepartmentId == departmentId).ToListAsync();
        }


        public async Task<List<Teacher>?> GetTeachersAsync()
        {
            return await _context.Teachers.Include(t => t.User).Include(t => t.Department).Include(t => t.Students).ToListAsync();
        }
    }
}
