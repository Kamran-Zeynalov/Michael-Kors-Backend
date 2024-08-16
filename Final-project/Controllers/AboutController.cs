using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index() 
        {
            return View();
        }
    }
}
