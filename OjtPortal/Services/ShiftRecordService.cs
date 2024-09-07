using OjtPortal.Infrastructure;

namespace OjtPortal.Services
{
    public interface IShiftRecordService
    {
    }

    public class ShiftRecordService : IShiftRecordService
    {
        private readonly ILogger<ShiftRecordService> _logger;
        private readonly IHolidayService _holidayService;

        public ShiftRecordService(ILogger<ShiftRecordService> logger, IHolidayService holidayService)
        {
            this._logger = logger;
            this._holidayService = holidayService;
        }

        
    }
}
