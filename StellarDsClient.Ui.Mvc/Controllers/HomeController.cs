using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Ui.Mvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Ui.Mvc.Providers;

//TODO
//TODO: Clear finished/completed lists

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [Authorize]
    [ProvideOAuthBaseAddress]
    public class HomeController(DataApiService<OAuthTokenProvider> dataApiService, TableSettings tableSettings) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var stellarDsResult = await dataApiService.GetLastUpdatedList(tableSettings.ListTableId, tableSettings.TaskTableId);

            return View(await stellarDsResult.ToHomeViewModel(dataApiService.DownloadBlobFromApi));
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
