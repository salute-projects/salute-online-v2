using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO;

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
    }
}
