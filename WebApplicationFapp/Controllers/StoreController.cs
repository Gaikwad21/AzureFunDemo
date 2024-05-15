using Microsoft.AspNetCore.Mvc;

namespace WebApplicationFapp.Controllers
{
    public class StoreController : Controller
    {
        [Route("store/books")]
        public IActionResult Books()
        {
            //return View();
            return Content("<h1>Books Store </h1>","text/html");
        }
    }
}
