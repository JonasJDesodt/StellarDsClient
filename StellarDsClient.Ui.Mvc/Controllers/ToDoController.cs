using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Models.Request;
using StellarDsClient.Models.Result;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Providers;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [Authorize]
    [Route("todo/{listId:int}")]
    public class ToDoController(DataApiService<ReadonlyAccessTokenProvider> readOnlyDataApiService, DataApiService<OAuthAccessTokenProvider> oAuthDataApiService) : Controller
    {
        [HttpGet]
        [Route("index")]
        [StoreQueryString]
        [ProvideQueryString(nameof(ListController))]
        public async Task<IActionResult> Index([FromRoute] int listId, [FromQuery] TaskIndexFilter? taskIndexFilter, [FromQuery] Pagination? pagination)
        {
            pagination ??= new Pagination();

            taskIndexFilter ??= new TaskIndexFilter();
            taskIndexFilter.ListId = listId;

            var stellarDsResult = await readOnlyDataApiService.GetListWithTasks(listId, pagination, taskIndexFilter);
            
            return View(stellarDsResult.ToTaskIndexViewModel(pagination, taskIndexFilter));
        }

        [HttpGet]
        [Route("create")]
        [ProvideQueryString(nameof(ToDoController))]
        public async Task<IActionResult> Create([FromRoute] int listId)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            return View(await stellarDsListResult.ToTaskCreateEditViewModel(oAuthDataApiService.DownloadBlobFromApi));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        [ProvideQueryString(nameof(ToDoController))]
        public async Task<IActionResult> Create([FromRoute] int listId, [FromForm] ToDoFormModel taskFormModel)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            if (!ModelState.IsValid)
            {
                return View(await taskFormModel.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));
            }

            await oAuthDataApiService.Create<CreateTaskRequest, ToDoResult>(nameof(ToDo), taskFormModel.ToCreateRequest(listId));
            //todo: error if no success?

            var queryParams = HttpUtility.ParseQueryString(HttpContext.Session.GetString(nameof(ToDoController)) ?? "").ToDictionary();
            queryParams.Add("ListId", $"{listId}");

            return RedirectToAction("Index", queryParams);
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        [ProvideQueryString(nameof(ToDoController))]
        public async Task<IActionResult> Edit([FromRoute] int listId, [FromRoute] int id)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            var stellarDsTaskResult = await oAuthDataApiService.Get<ToDoResult>(nameof(ToDo), id);

            return View(await stellarDsTaskResult.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit/{id:int}")]
        [ProvideQueryString(nameof(ToDoController))]
        public async Task<IActionResult> Edit([FromRoute] int listId, [FromRoute] int id, [FromForm] ToDoFormModel taskFormModel)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            if (!ModelState.IsValid)
            {
                return View(await taskFormModel.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));

            }

            await oAuthDataApiService.Put<PutTaskRequest, ToDoResult>(nameof(ToDo), id, taskFormModel.ToPutRequest());
            //todo: error if no success?

            var queryParams = HttpUtility.ParseQueryString(HttpContext.Session.GetString(nameof(ToDoController)) ?? "").ToDictionary();
            queryParams.Add("ListId", $"{listId}");

            return RedirectToAction("Index", queryParams);
        }

        [HttpGet]
        [Route("delete-request/{id:int}")]
        [ProvideQueryString(nameof(ToDoController))]
        public async Task<IActionResult> DeleteRequest([FromRoute] int listId, [FromRoute] int id)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            if ((await oAuthDataApiService.Get<ToDoResult>(nameof(ToDo), id)) is not { } stellarDbResult)
            {
                return RedirectToAction("Index", "ToDo", new { listId }); // todo: error page
            }

            return View("Edit", await stellarDbResult.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi,true));
        }

        [HttpGet]
        [Route("delete/{id:int}")]
        [ProvideQueryString(nameof(ToDoController))]
        public async Task<IActionResult> Delete([FromRoute] int listId, [FromRoute] int id)
        {
            if ((await oAuthDataApiService.Get<ToDoResult>(nameof(ToDo), id)).Data is {} taskResult && taskResult.ListId == listId)
            {
                await oAuthDataApiService.Delete(nameof(ToDo), id);
            }

            var queryParams = HttpUtility.ParseQueryString(HttpContext.Session.GetString(nameof(ToDoController)) ?? "").ToDictionary();
            queryParams.Add("ListId", $"{listId}");

            return RedirectToAction("Index", queryParams);
        }
    }
}