using System.Net;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.ConfigurationService.Controllers
{
    [Produces("application/json")]
    [Route("api/Base")]
    public class BaseController : Controller
    {
        protected ObjectResult ProcessExceptionResult(SoException e)
        {
            switch (e.HttpCode)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest(e.Message);
                case HttpStatusCode.Unauthorized:
                    return new ObjectResult(e.Message)
                    {
                        StatusCode = (int) e.HttpCode
                    };
                case HttpStatusCode.NotFound:
                    return NotFound(e.Message);
                case HttpStatusCode.InternalServerError:
                    return new ObjectResult(e.Message)
                    {
                        StatusCode = (int)e.HttpCode
                    };
                default:
                    return new ObjectResult(e.Message)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
            }
        }
    }
}