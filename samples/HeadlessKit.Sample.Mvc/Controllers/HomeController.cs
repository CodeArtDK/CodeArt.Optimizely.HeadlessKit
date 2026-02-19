using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HeadlessKit.Sample.Mvc.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
