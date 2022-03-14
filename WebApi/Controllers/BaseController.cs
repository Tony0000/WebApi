using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        protected IActionResult NotFound<T>()
        {
            return NotFound($"{typeof(T).Name} not found.");
        }
    }
}