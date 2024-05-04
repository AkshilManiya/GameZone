using Microsoft.AspNetCore.Mvc;

namespace GameZoneManagment.Controllers
{
    [Route("/Manager")]
    public class Manager : Controller
    {
        [Route("Home")]
        public IActionResult HomeView()
        {
            return View("ManagerHome");
        }

        [Route("Profile")]
        public IActionResult ProfileView()
        {
            return View("ManagerProfile");
        }
    }
}
