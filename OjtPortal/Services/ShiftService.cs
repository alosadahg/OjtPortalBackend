using OjtPortal.Infrastructure;

namespace OjtPortal.Services
{
    public interface IShiftRecordService
    {
    }

    public class ShiftService : IShiftRecordService
    {
        private readonly ILogger<ShiftService> _logger;
        private readonly IHolidayService _holidayService;

        public ShiftService(ILogger<ShiftService> logger, IHolidayService holidayService)
        {
            this._logger = logger;
            this._holidayService = holidayService;
        }

        
    }
}
