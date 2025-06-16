using Microsoft.AspNetCore.Mvc;

namespace kopinang_api.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected string? GetUid()
        {
            return HttpContext.Items["uid"]?.ToString();
        }

        protected IActionResult FirebaseUnauthorized()
        {
            return Unauthorized("Token Firebase tidak valid");
        }
    }
}
