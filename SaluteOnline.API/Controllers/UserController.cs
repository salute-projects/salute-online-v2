using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IAccountService _accountService;

        public UserController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetUserInfo()
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");

                var user = _accountService.GetUserInfo(email);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult UpdateUserInfo([FromBody]UserDto userDto)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                _accountService.UpdateUserInfo(userDto, email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult UpdateMainUserInfo([FromBody]UserMainInfoDto userDto)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                _accountService.UpdateMainUserInfo(userDto, email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult UpdatePersonalUserInfo([FromBody]UserPersonalInfoDto userDto)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                _accountService.UpdatePersonalUserInfo(userDto, email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("uploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(await _accountService.UpdateUserAvatar(avatar, email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
