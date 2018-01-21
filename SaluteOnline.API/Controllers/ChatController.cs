using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Chat;
using SaluteOnline.Domain.Exceptions;

namespace SaluteOnline.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Chat")]
    public class ChatController : BaseController
    {
        private readonly IChatService _service;

        public ChatController(IChatService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetMyChats()
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized();
                return Ok(_service.GetMyChats(email));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost]
        public IActionResult PostPrivateMessage([FromBody]PostPrivateMessageDto dto)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized();
                _service.PostPrivateMessage(dto);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpGet("latest/{take:int}")]
        public IActionResult GetLatest(int take)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized();
                return Ok(_service.GetLatestMessages(take, email));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpGet("{chatGuid:guid}/{page:int}/{pageSize:int}")]
        public IActionResult GetChatMessages(Guid chatGuid, int page, int pageSize)
        {
            try
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Authorization failed.");
                return Ok(_service.LoadChatMessages(new BaseFilter
                {
                    Page = page,
                    PageSize = pageSize
                }, chatGuid, email));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

    }
}