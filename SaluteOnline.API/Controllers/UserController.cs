using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.DTO.User;
using SaluteOnline.API.Infrastructure.Kafka;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Events;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IKafkaProducer _produce;

        public UserController(IAccountService accountService, IKafkaProducer producer)
        {
            _accountService = accountService;
            _produce = producer;
        }

        [HttpGet("kafka")]
        public async Task<IActionResult> TestKafka()
        {
            var result = await _produce.ProduceAsync(new UserCreated
            {
                UserId = 1,
                SubjectId = "id"
            });
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult GetUserInfo()
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
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
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult UpdateUserInfo([FromBody]UserDto userDto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
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
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult UpdateMainUserInfo([FromBody]UserMainInfoDto userDto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
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
        [Authorize(Policy = nameof(Policies.User))]
        public IActionResult UpdatePersonalUserInfo([FromBody]UserPersonalInfoDto userDto)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
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
        [Authorize(Policy = nameof(Policies.User))]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
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
