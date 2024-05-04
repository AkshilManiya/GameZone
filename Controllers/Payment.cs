using Microsoft.AspNetCore.Mvc;

namespace GameZoneManagment.Controllers
{
    [Route("Pay")]
    public class Payment : Controller
    {
        [Route("add")]
        public IActionResult Index()
        {
            return View("../Member/MyWallet");
        }
    }
}
