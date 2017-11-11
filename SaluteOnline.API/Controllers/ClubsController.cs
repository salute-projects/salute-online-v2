using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Club;

namespace SaluteOnline.API.Controllers
{
    [Route("api/[controller]")]
    public class ClubsController : Controller
    {
        private readonly IClubsService _service;
        public ClubsController(IClubsService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public async Task<IActionResult> CreateClub([FromBody]CreateClubDto club)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(await _service.CreateClub(club, email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("list")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetClubsList([FromBody] ClubFilter filter)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.GetClubs(filter, email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetClubInfoAggregation()
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.GetInfoAggregation());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetClubInfo(int id)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.GetClub(id, email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("admins")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetClubAdministrators([FromBody] ClubMembersFilter filter)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.GetClubAdministrators(filter));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("members")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetClubMembers([FromBody] ClubMembersFilter filter)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.GetClubMembers(filter));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("addClubMember")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult AddClubMember([FromBody] CreateClubMemberDto dto)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.AddClubMember(dto, email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("addMembershipRequest")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult AddMembershipRequest([FromBody] MembershipRequestCreateDto dto)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.AddMembershipRequest(dto, email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("getMembershipRequests")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetMembershipRequests([FromBody] EntityFilter filter)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.GetClubMembershipRequests(filter, email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
