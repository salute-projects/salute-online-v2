using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp([FromBody]UserEssential user)
        {
            try
            {
                return Ok(await _accountService.SignUp(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserEssential user)
        {
            try
            {
                return Ok(await _accountService.Login(user));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
