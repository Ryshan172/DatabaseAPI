#pragma warning disable CS1591
#pragma warning disable CS8602

using Microsoft.AspNetCore.Diagnostics;

namespace DatabaseApiCode.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {   
        [HttpGet("/error")]
        [HttpPut("/error")]
        [HttpPost("/error")]
        public IActionResult Error()

        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var stackTrace = context.Error.StackTrace;
            var errorMessage = context.Error.Message;
            return Problem();
        }

    }
}