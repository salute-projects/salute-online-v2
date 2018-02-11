using System;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;

namespace SaluteOnline.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("userExists/{email}")]
        public IActionResult UserExists(string email)
        {
            try
            {
                return Ok(_accountService.UserExists(email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}