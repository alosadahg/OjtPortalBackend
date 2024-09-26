using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IOtpRepo
    {
        Task<OTP?> AddOTPAsync(OTP newOTP);
        Task<OTP?> GetOTPByIdAsync(int id);
        Task<bool> IsOTPExisting(OTP otp);
        Task RemoveOTP(OTP otp);
    }

    public class OtpRepo : IOtpRepo
    {
        private readonly OjtPortalContext _context;

        public OtpRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<OTP?> AddOTPAsync(OTP newOTP)
        {
            if (IsOTPExisting(newOTP).Result)   
            {
                var otp = await GetOTPByIdAsync(newOTP.UserId);
                otp!.Code = newOTP.Code;
                otp!.Token= newOTP.Token;
                //_context.Entry(otp).State = EntityState.Modified;
            } 
            else
            {
                await _context.OTPs.AddAsync(newOTP);
            }
            _context.Entry(newOTP.User!).State = EntityState.Unchanged;
            await _context.SaveChangesAsync();
            return newOTP;
        }

        public async Task<bool> IsOTPExisting(OTP otp)
        {
            return await _context.OTPs.ContainsAsync(otp);
        }

        public async Task<OTP?> GetOTPByIdAsync(int id)
        {
            return await _context.OTPs.FirstOrDefaultAsync(otp => otp.UserId == id);
        }
        public async Task RemoveOTP(OTP otp)
        {
            _context.OTPs.Remove(otp);
            await _context.SaveChangesAsync();
        }

    }
}
