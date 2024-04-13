using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Builder.Library.Models;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Ui.Mvc.Providers;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [Authorize]
    [Route("lists")]
    [ProvideOAuthBaseAddress]
    public class ListController(DataApiService<ReadonlyAccessTokenProvider> readOnlyDataApiService, DataApiService<OAuthTokenProvider> oAuthDataApiService) : Controller
    {
        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index([FromQuery] ListIndexFilter? listIndexFilter, [FromQuery] Pagination? pagination)
        {
            pagination ??= new Pagination();

            //todo; does not seem to work with pagination (offset/take)
            //var joinQuery = "&joinQuery=" + $"task;ListId=list;Id";

            if (listIndexFilter?.Scoped is true)
            {
                var stellarDsResult = await oAuthDataApiService.Find<ListResult>(nameof(List), listIndexFilter.GetQuery() + pagination.GetQuery());

                return View(await stellarDsResult.ToListIndexViewModel(readOnlyDataApiService.DownloadBlobFromApi, listIndexFilter, pagination));
            }
            else
            {
                var stellarDsResult = await readOnlyDataApiService.Find<ListResult>(nameof(List), listIndexFilter.GetQuery() + pagination.GetQuery());

                return View(await stellarDsResult.ToListIndexViewModel(readOnlyDataApiService.DownloadBlobFromApi, listIndexFilter, pagination));
            }
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View(new ListCreateEditViewModel());
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ListFormModel listFormModel)
        {
            if (!ModelState.IsValid)
            {
                return View(new ListCreateEditViewModel { ListFormModel = listFormModel });
            }

            if (User.GetId() is not { } ownerId || User.GetName() is not { } ownerName)
            {
                return RedirectToAction("SignOut", "OAuth");//todo: add message?
            }

            return RedirectToAction("Index", "ToDo", new { ListId = await oAuthDataApiService.CreateListWithBlob(listFormModel, ownerId, ownerName) });
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var stellarDsResult = await oAuthDataApiService.Get<ListResult>(nameof(List), id);

            return View(await stellarDsResult.ToListCreateEditViewModel(readOnlyDataApiService.DownloadBlobFromApi));
        }

        [HttpPost]
        [Route("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ListFormModel listFormModel)
        {
            if (!ModelState.IsValid)
            {
                return View(listFormModel.ToListCreateEditViewModel());
            }

            await oAuthDataApiService.UpdateListWithBlob(id, listFormModel);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("delete-request/{id:int}")]
        public async Task<IActionResult> DeleteRequest([FromRoute] int id)
        {
            var stellarDsResult = await oAuthDataApiService.Get<ListResult>(nameof(List), id);

            return View("Edit", await stellarDsResult.ToListCreateEditViewModel(readOnlyDataApiService.DownloadBlobFromApi, true));
        }

        [HttpGet]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await oAuthDataApiService.DeleteListWithTasks(id);

            return RedirectToAction("Index");
        }
    }
}