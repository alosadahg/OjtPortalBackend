﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
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
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(SubMentorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPost]
        public async Task<IActionResult> RegisterSubmentor([Required] int mentorId,[Required] int submentorId)
        {
            var (result, error) = await _subMentorService.RegisterSubmentor(mentorId, submentorId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Gets the submentors by headmentor
        /// </summary>
        /// <param name="headMentorId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FullMentorDtoWithSubMentors))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("headmentor/{headMentorId}")]
        public async Task<IActionResult> GetSubmentorsByHeadMentorAsync([Required] int headMentorId)
        {
            var (result, error) = await _subMentorService.GetSubmentorsByHeadMentorIdAsync(headMentorId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Gets all mentors available to be submentors
        /// </summary>
        /// <param name="companyId">The id of the company</param>
        /// <returns></returns>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableForSubmentorsAsync([Required] int companyId)
        {
            var (result, error) = await _subMentorService.GetMentorsWithNoHeadMentorsAsync(companyId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Delegate task to submentor [Requires Auth}
        /// </summary>
        /// <param name="submentorId">The id of the submentor</param>
        /// <param name="taskId">The id of the task</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("assign/task/")]
        public async Task<IActionResult> AssignSubmentorTaskAsync([Required] int submentorId, [Required] int taskId)
        {
            var user = await _userManager.GetUserAsync(User);
            var (result, error) = await _subMentorService.DelegateSubmentorToTaskAsync(user!.Id, submentorId, taskId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}
