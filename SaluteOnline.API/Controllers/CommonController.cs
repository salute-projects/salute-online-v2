using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.API.Services.Interface;

namespace SaluteOnline.API.Controllers
{
    [Route("api/[controller]")]
    public class CommonController : Controller
    {
        private readonly ICommonService _service;

        public CommonController(ICommonService service)
        {
            _service = service;
        }

        [HttpGet("countries")]
        public IActionResult GetCountiesList()
        {
            try
            {
                return Ok(_service.GetContriesList());
            }
            catch (Exception)
            {
                return BadRequest("Error while retrieving list of counties");
            }
        }
    }
}
