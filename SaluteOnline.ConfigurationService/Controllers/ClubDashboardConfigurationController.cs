using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.ConfigurationService.Domain;
using SaluteOnline.ConfigurationService.Service.Declaration;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.ConfigurationService.Controllers
{
    [Produces("application/json")]
    [Route("api/clubDashboardConfiguration")]
    public class ClubDashboardConfigurationController : BaseController
    {
        private readonly IConfigurationService _service;

        public ClubDashboardConfigurationController(IConfigurationService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetConfiguration(int id)
        {
            try
            {
                var subjectId = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(subjectId))
                    return BadRequest("Authorization failed.");

                var role = User.Claims.SingleOrDefault(t => t.Type == "role")?.Value;
                if (string.IsNullOrEmpty(role))
                    return BadRequest("Authorization failed.");

                return Ok(_service.GetClubDashboardConfiguration(subjectId, role, id));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult SaveConfiguration([FromBody] ClubDashboardConfiguration configuration)
        {
            try
            {
                var id = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Authorization failed.");

                _service.SaveClubDashboardConfiguration(configuration, id);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}