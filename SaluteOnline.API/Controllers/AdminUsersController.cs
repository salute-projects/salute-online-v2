using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.DTO.User;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Controllers
{
    [Produces("application/json")]
    [Route("api/adminUsers")]
    public class AdminUsersController : BaseController
    {
        private readonly IUsersService _service;

        public AdminUsersController(IUsersService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = nameof(Policies.GlobalAdmin))]
        public IActionResult GetUsers([FromQuery] UserFilter request)
        {
            try
            {
                return Ok(_service.GetUsers(request));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPut("setRole")]
        [Authorize(Policy = nameof(Policies.GlobalAdmin))]
        public IActionResult SetUserRole([FromBody] SetRoleRequest request)
        {
            try
            {
                _service.SetUserRole(request);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPut("setStatus")]
        [Authorize(Policy = nameof(Policies.GlobalAdmin))]
        public IActionResult SetUserStatus([FromBody] SetStatusRequest request)
        {
            try
            {
                _service.SetUserStatus(request);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}