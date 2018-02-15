using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.ChatService.Domain.DTO;
using SaluteOnline.ChatService.Service.Abstraction;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.Exceptions;

namespace SaluteOnline.ChatService.Controllers
{
    [Produces("application/json")]
    [Route("api/chat")]
    public class ChatController : BaseController
    {
        private readonly IChatService _service;

        public ChatController(IChatService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetMyChats()
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "subjectId")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return Unauthorized();

                return Ok(_service.GetMyChats(subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult PostPrivateMessage([FromBody]PostPrivateMessageDto dto)
        {
            try
            {
                _service.PostPrivateMessage(dto);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpGet("latest/{take:int}")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetLatest(int take)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "subjectId")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return Unauthorized();

                return Ok(_service.GetLatestMessages(take, subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpGet("{chatGuid:guid}/{page:int}/{pageSize:int}")]
        [Authorize(AuthenticationSchemes = "Auth", Policy = nameof(Policies.User))]
        public IActionResult GetChatMessages(Guid chatGuid, int page, int pageSize)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "subjectId")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                return Ok(_service.LoadChatMessages(new BaseFilter
                {
                    Page = page,
                    PageSize = pageSize
                }, chatGuid, subjectId));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}