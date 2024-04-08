using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Ui.Mvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using StellarDsClient.Ui.Mvc.Attributes;

//TODO
//Clear finished/completed lists

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [ProvideOAuthBaseAddress]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //get list with the task edited or created last
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
