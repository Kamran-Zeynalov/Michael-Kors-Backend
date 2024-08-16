using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
