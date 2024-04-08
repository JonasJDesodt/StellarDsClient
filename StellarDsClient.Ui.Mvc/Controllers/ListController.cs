using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Dto.Data.Request;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Ui.Mvc.Providers;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [Authorize]
    [Route("lists")]
    [ProvideOAuthBaseAddress]
    public class ListController(DataApiService<AccessTokenProvider> readOnlyDataApiService, DataApiService<OAuthTokenProvider> oAuthDataApiService, TableSettings tableSettings) : Controller
    {
        private readonly int _listTableId = tableSettings.ListTableId;
        private readonly int _taskTableId = tableSettings.TaskTableId;

        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index([FromQuery] ListIndexFilter? listIndexFilter, [FromQuery] Pagination? pagination)
        {
            pagination ??= new Pagination();

            //todo; does not seem to work with pagination (offset/take)
            //var joinQuery = "&joinQuery=" + $"task;ListId=list;Id";

            if (listIndexFilter?.Scoped is true)
            {
                var stellarDsResult = await oAuthDataApiService.Find<ListResult>(_listTableId, listIndexFilter.GetQuery() + pagination.GetQuery());

                return View(await stellarDsResult.ToListIndexViewModel(readOnlyDataApiService.DownloadBlobFromApi, listIndexFilter, pagination));
            }
            else
            { 
               var stellarDsResult = await readOnlyDataApiService.Find<ListResult>(_listTableId, listIndexFilter.GetQuery() + pagination.GetQuery());

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

            if ((await oAuthDataApiService.Create<CreateListRequest, ListResult>(_listTableId, listFormModel.ToCreateListRequest(ownerId, ownerName))).Data is not { } listResult)
            {
                return View(new ListCreateEditViewModel { ListFormModel = listFormModel }); //todo error view?
            };

            if (listFormModel.ImageUpload is null)
            {
                return RedirectToAction("Index", "Task", new { listId = listResult.Id });
            }

            using var multipartFormDataContent = new MultipartFormDataContent().AddFormFile(listFormModel.ImageUpload);

            await oAuthDataApiService.UploadFileToApi(_listTableId, "image", listResult.Id, multipartFormDataContent);

            return RedirectToAction("Index", "Task", new { ListId = listResult.Id });
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var stellarDsResult = await oAuthDataApiService.Get<ListResult>(_listTableId, id);

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

            await oAuthDataApiService.Put<PutListRequest, ListResult>(_listTableId, id, listFormModel.ToPutListRequest());

            if (listFormModel.ImageUpload is null)
            {
                return RedirectToAction("Index");
            }

            using var multipartFormDataContent = new MultipartFormDataContent().AddFormFile(listFormModel.ImageUpload);

            await oAuthDataApiService.UploadFileToApi(_listTableId, "image", id, multipartFormDataContent);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("delete-request/{id:int}")]
        public async Task<IActionResult> DeleteRequest([FromRoute] int id)
        {
            var stellarDsResult = await oAuthDataApiService.Get<ListResult>(_listTableId, id);
            
            return View("Edit", await stellarDsResult.ToListCreateEditViewModel(readOnlyDataApiService.DownloadBlobFromApi, true));
        }

        [HttpGet]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if ((await oAuthDataApiService.Find<TaskResult>(_taskTableId, $"&whereQuery=ListId;equal;{id}")).Data is not { } taskResults)
            {
                return RedirectToAction("Index"); //todo error? do not allow to delete the list without deleting tasks possibly associated with it
            }

            await oAuthDataApiService.Delete(_taskTableId, taskResults.Select(taskResult => taskResult.Id.ToString()).ToArray());

            await oAuthDataApiService.Delete(_listTableId, id);

            return RedirectToAction("Index");
        }
    }
}