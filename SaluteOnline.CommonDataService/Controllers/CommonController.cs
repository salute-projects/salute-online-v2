using Microsoft.AspNetCore.Mvc;
using SaluteOnline.CommonDataService.Service.Declaration;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.CommonDataService.Controllers
{
    [Route("api/[controller]")]
    public class CommonController : BaseController
    {
        private readonly ICommonService _service;

        public CommonController(ICommonService service)
        {
            _service = service;
        }

        [HttpGet("countries"), ResponseCache(CacheProfileName = "CachingProfile")]
        public IActionResult GetCountiesList()
        {
            try
            {
                return Ok(_service.GetContriesList());
            }
            catch (SoException e)
            {
                return ProcessExceptionResult(e);
            }
        }
    }
}
