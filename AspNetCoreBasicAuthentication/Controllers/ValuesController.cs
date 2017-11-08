using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreBasicAuthentication.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ValuesController : Controller
    {
        public IActionResult Values()
        {
            var values = new[] {"You", "are", "awesome!"};
            return Ok(values);
        }
    }
}
