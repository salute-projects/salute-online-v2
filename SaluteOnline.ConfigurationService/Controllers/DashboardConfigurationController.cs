using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.ConfigurationService.Domain;
using SaluteOnline.ConfigurationService.Service.Declaration;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.ConfigurationService.Controllers
{
    [Route("api/dashboardConfiguration")]
    public class DashboardConfigurationController : BaseController
    {
        private readonly IConfigurationService _service;

        public DashboardConfigurationController(IConfigurationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetConfiguration()
        {
            try
            {
                var id = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Authorization failed.");

                var role = User.Claims.SingleOrDefault(t => t.Type == "role")?.Value;
                if (string.IsNullOrEmpty(role))
                    return BadRequest("Authorization failed.");

                return Ok(_service.GetDashboardConfiguration(id, role));
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult SaveConfiguration([FromBody] List<DashboardConfigurationItem> panels)
        {
            try
            {
                var id = User.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Authorization failed.");

                _service.SaveDashboardConfiguration(panels, id);
                return Ok();
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}