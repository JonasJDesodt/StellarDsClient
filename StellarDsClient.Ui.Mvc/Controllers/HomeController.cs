using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Ui.Mvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Ui.Mvc.Providers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Diagnostics;
using StellarDsClient.Sdk.Exceptions;

//TODO
//TODO: Clear finished/completed lists

//bug: list last worked on only appears if the list has a task

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [Authorize]
    public class HomeController(DataApiService<OAuthAccessTokenProvider> dataApiService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var stellarDsResult = await dataApiService.GetLastUpdatedList();

            return View(await stellarDsResult.ToHomeViewModel(dataApiService.DownloadBlobFromApi));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            switch (exception)
            {
                case CustomNotFoundException:
                    ViewBag.Message = exception.Message;
                    return View("NotFound");
                case CustomUnauthorizedException:
                    ViewBag.Message = exception.Message;
                    return RedirectToAction("SignOut", "OAuth", new { returnUrl = "/" });
                case CustomHttpException:
                    ViewBag.Message = exception.Message;
                    return View("GeneralError");
                default:
                    break;
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}