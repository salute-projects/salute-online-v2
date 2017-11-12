using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.InnerMessage;

namespace SaluteOnline.API.Controllers
{
    [Route("api/[controller]")]
    public class InnerMessagesController : Controller
    {
        private readonly IMessageService _service;
        public InnerMessagesController(IMessageService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetMessages([FromBody]InnerMessagesFilter filter)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.GetMessages(filter, email));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}