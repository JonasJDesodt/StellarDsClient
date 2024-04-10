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
            var pagination = new Pagination
            {
                Page = 1,
                PageSize = 1
            };

            //todo: use join?

            var listIndexFilter = new ListIndexFilter
            {
                Sort = "updated"
            };
            
            var listStellarDsResult = await dataApiService.Find<ListResult>(tableSettings.ListTableId, listIndexFilter.GetQuery() + pagination.GetQuery());
            if (listStellarDsResult.Data?.FirstOrDefault() is not { } listResult)
            {
                return View(new HomeViewModel());
            }
            

            var taskIndexFilter = new TaskIndexFilter
            {
                Sort = "updated"
            };

            var taskStellarDsResult = await dataApiService.Find<TaskResult>(tableSettings.TaskTableId, taskIndexFilter.GetQuery() + pagination.GetQuery());

            if (taskStellarDsResult.Data?.FirstOrDefault() is not { } taskResult)
            {
                return View(new HomeViewModel());
            }
            
            StellarDsResult<ListResult>? stellarDsResult;

            if (taskResult.Updated > listResult.Updated)
            {
                stellarDsResult = await dataApiService.Get<ListResult>(tableSettings.ListTableId, taskResult.ListId);
            }
            else
            {
                stellarDsResult = new StellarDsResult<ListResult>
                {
                    Data = listResult
                };
            }
          
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
