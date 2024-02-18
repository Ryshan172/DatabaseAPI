using Microsoft.AspNetCore.Diagnostics;

namespace DatabaseApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {   
        [HttpPut("/error")]
        [HttpPost("/error")]
        [HttpGet("/error")]
        public IActionResult Error()

        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var stackTrace = context.Error.StackTrace;
            var errorMessage = context.Error.Message;
            return Problem();
        }

    }
}