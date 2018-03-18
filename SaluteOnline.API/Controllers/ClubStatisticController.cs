using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Controllers
{
    [Produces("application/json")]
    [Route("api/clubStatistic")]
    public class ClubStatisticController : BaseController
    {
        private readonly IStatisticService _service;
        public ClubStatisticController(IStatisticService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = nameof(Policies.ClubAdmin))]
        public IActionResult MyClubsStatistic()
        {
            try
            {
                var id = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Authorization failed.");

                return Ok(_service.GetMyClubsStatistics(id));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}