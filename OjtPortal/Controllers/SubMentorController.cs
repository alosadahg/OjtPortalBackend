using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Entities;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/submentor")]
    public class SubMentorController : OjtPortalBaseController
    {
        private readonly ISubMentorService _subMentorService;
        private readonly UserManager<User> _userManager;

        public SubMentorController(ISubMentorService subMentorService, UserManager<User> userManager)
        {
            this._subMentorService = subMentorService;
            this._userManager = userManager;
        }

        /// <summary>
        /// Add submentor 
        /// </summary>
        /// <param name="mentorId">The head mentor's id</param>
        /// <param name="submentorId">The submentor's id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RegisterSubmentor(int mentorId, int submentorId)
        {
            var (result, error) = await _subMentorService.RegisterSubmentor(mentorId, submentorId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}
