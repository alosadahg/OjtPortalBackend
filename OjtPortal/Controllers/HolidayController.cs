using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/holidays")]
    public class HolidayController : OjtPortalBaseController
    {
        private readonly HolidayService _holidayService;

        public HolidayController(HolidayService holidayService)
        {
            this._holidayService = holidayService;
        }

        /// <summary>
        /// Get the holidays in the Philippines
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetHolidays()
        {
            var (holidays, error) = await _holidayService.GetHolidaysAsync();
            if (error != null)  return MakeErrorResponse(error);
            return Ok(holidays);
        }
    }
}
