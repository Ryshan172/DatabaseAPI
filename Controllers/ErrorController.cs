using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace DatabaseApi.Controllers
{
    public class ErrorController : ControllerBase
    {
        [HttpPost("/error")]
        [HttpGet("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            // var stackTrace = context.Error.StackTrace;
            var errorMessage = context.Error.Message;
            return Problem(errorMessage);
        }

    }
}