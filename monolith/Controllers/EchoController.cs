using Microsoft.AspNetCore.Mvc;

namespace Optivem.AtddAccelerator.Template.Monolith.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EchoController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello from API!");
    }
}
