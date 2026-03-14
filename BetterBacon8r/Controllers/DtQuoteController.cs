using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace AlsProjects.Controllers {
    public class DtQuoteController : Controller {

        public DtQuoteController() {
        }

        public IActionResult Index() {
            return View(new List<string>());
        }
    }
}
