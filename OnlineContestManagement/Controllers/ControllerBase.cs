using Microsoft.AspNetCore.Mvc;

namespace OnlineContestManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        // Add common controller logic here if needed
    }
}