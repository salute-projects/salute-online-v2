﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.DTO.Club;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Controllers
{
    [Route("api/[controller]")]
    public class ClubsController : BaseController
    {
        private readonly IClubsService _service;
        public ClubsController(IClubsService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Policy = nameof(Policies.User))]
        public async Task<IActionResult> CreateClub([FromBody]CreateClubDto club)
        {
            try
            {
                var id = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Authorization failed.");

                return Ok(await _service.CreateClub(club, id));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("list")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult GetClubsList([FromBody] ClubFilter filter)
        {
            try
            {
                var id = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Authorization failed.");

                return Ok(_service.GetClubs(filter, id));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpGet("myList")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult GetMyClubs()
        {
            try
            {
                var id = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Authorization failed.");

                return Ok(_service.GetMyClubs(id));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpGet]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult GetClubInfoAggregation()
        {
            try
            {
                return Ok(_service.GetInfoAggregation());
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult GetClubInfo(int id)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                return Ok(_service.GetClub(id, subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("admins")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult GetClubAdministrators([FromBody] ClubMembersFilter filter)
        {
            try
            {
                return Ok(_service.GetClubAdministrators(filter));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("members")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult GetClubMembers([FromBody] ClubMembersFilter filter)
        {
            try
            {
                return Ok(_service.GetClubMembers(filter));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("addClubMember")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult AddClubMember([FromBody] CreateClubMemberDto dto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                return Ok(_service.AddClubMember(dto, subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("addMembershipRequest")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult AddMembershipRequest([FromBody] MembershipRequestCreateDto dto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                return Ok(_service.AddMembershipRequest(dto, subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("getMembershipRequests")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult GetMembershipRequests([FromBody] MembershipRequestFilter filter)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                return Ok(_service.GetClubMembershipRequests(filter, subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("handleMembershipRequest")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult HandleMembershipRequest([FromBody] HandleMembershipRequestDto dto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                _service.HandleMembershipRequest(dto, subjectId);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpGet("canRegisterClub")]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult CanRegisterClub()
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                return Ok(_service.CanRegisterClub(subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("changeAvatar/{id}")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar, int id)
        {
            try
            {
                return Ok(await _service.ChangeAvatar(avatar, id));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}
