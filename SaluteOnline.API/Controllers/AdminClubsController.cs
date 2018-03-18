using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.DTO.Club;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Controllers
{
    [Produces("application/json")]
    [Route("api/adminClubs")]
    public class AdminClubsController : BaseController
    {
        private readonly IClubsService _service;

        public AdminClubsController(IClubsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = nameof(Policies.GlobalAdmin))]
        public IActionResult GetClubsForAdmin([FromQuery] ClubFilter filter)
        {
            try
            {
                return Ok(_service.GetClubsForAdministration(filter));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPut("changeStatus")]
        [Authorize(Policy = nameof(Policies.GlobalAdmin))]
        public IActionResult ChangeClubStatus([FromBody] ClubChangeStatusRequest request)
        {
            try
            {
                _service.ChangeClubStatus(request);
                return Ok("");
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}