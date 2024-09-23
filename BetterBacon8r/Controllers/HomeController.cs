using Microsoft.AspNetCore.Mvc;

namespace BetterBacon8r.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
