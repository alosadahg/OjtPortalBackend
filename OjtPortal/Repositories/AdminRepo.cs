using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;
using OjtPortal.Enums;

namespace OjtPortal.Repositories
{
    public interface IAdminRepo
    {
        Task<Admin?> GetAdminByIdAsync(int id);
        Task<bool> IsAdminExisting(Admin admin);
    }

    public class AdminRepo : IAdminRepo
    {
        private readonly OjtPortalContext _context;
        private readonly IMapper _mapper;

        public AdminRepo(OjtPortalContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<bool> IsAdminExisting(Admin admin)
        {
            return await _context.Users.ContainsAsync(admin);
        }

        public async Task<Admin?> GetAdminByIdAsync(int id)
        {
            var result = await _context.Users.Where(u => u.Id == id).Where(u => u.UserType== UserType.Admin).FirstOrDefaultAsync();
            return (_mapper.Map<Admin>(result));
        }
    }
}
