using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [Route("api/shift/records")]
    [ApiController]
    public class ShiftRecordController : OjtPortalBaseController
    {
        private readonly IShiftRecordService _shiftRecordService;

        public ShiftRecordController(IShiftRecordService shiftRecordService)
        {
            this._shiftRecordService = shiftRecordService;
        }

    }
}
