using Microsoft.AspNetCore.Mvc;

namespace Sav.WebApp.Controllers
{
    public class TestController : ControllerBase
    {

        public TestController() { }

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}
