using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.DTO.User;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseController
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
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "subjectId")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                var user = _accountService.GetUserInfo(subjectId);
                return Ok(user);
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult UpdateUserInfo([FromBody]UserDto userDto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "subjectId")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                _accountService.UpdateUserInfo(userDto, subjectId);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult UpdateMainUserInfo([FromBody]UserMainInfoDto userDto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "subjectId")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                _accountService.UpdateMainUserInfo(userDto, subjectId);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPatch]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult UpdatePersonalUserInfo([FromBody]UserPersonalInfoDto userDto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "subjectId")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                _accountService.UpdatePersonalUserInfo(userDto, subjectId);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost("uploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "subjectId")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                return Ok(await _accountService.UpdateUserAvatar(avatar, subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}
