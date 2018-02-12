using System.Net;
using Microsoft.AspNetCore.Mvc;
using SaluteOnline.Domain.Exceptions;

namespace SaluteOnline.ChatService.Controllers
{
    [Produces("application/json")]
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