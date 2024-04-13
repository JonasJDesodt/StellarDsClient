using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Builder.Library.Models;
using StellarDsClient.Dto.Data.Request;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Providers;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [Authorize]
    [Route("tasks/{listId:int}")]
    [ProvideOAuthBaseAddress]
    public class ToDoController(DataApiService<ReadonlyAccessTokenProvider> readOnlyDataApiService, DataApiService<OAuthTokenProvider> oAuthDataApiService) : Controller
    {
        [HttpGet]
        [Route("index")]
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
        public async Task<IActionResult> Create([FromRoute] int listId)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            return View(await stellarDsListResult.ToTaskCreateEditViewModel(oAuthDataApiService.DownloadBlobFromApi));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<IActionResult> Create([FromRoute] int listId, [FromForm] TaskFormModel taskFormModel)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            if (!ModelState.IsValid)
            {
                return View(await taskFormModel.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));
            }

            await oAuthDataApiService.Create<CreateTaskRequest, TaskResult>(nameof(ToDo), taskFormModel.ToCreateRequest(listId));
            //todo: error if no success?

            return RedirectToAction("Index", new { listId });
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] int listId, [FromRoute] int id)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            var stellarDsTaskResult = await oAuthDataApiService.Get<TaskResult>(nameof(ToDo), id);

            return View(await stellarDsTaskResult.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] int listId, [FromRoute] int id, [FromForm] TaskFormModel taskFormModel)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            if (!ModelState.IsValid)
            {
                return View(await taskFormModel.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));

            }

            await oAuthDataApiService.Put<PutTaskRequest, TaskResult>(nameof(ToDo), id, taskFormModel.ToPutRequest());
            //todo: error if no success?

            return RedirectToAction("Index", new { listId });
        }

        [HttpGet]
        [Route("delete-request/{id:int}")]
        public async Task<IActionResult> DeleteRequest([FromRoute] int listId, [FromRoute] int id)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>(nameof(List), listId);

            if ((await oAuthDataApiService.Get<TaskResult>(nameof(ToDo), id)) is not { } stellarDbResult)
            {
                return RedirectToAction("Index", "ToDo", new { listId }); // todo: error page
            }

            return View("Edit", await stellarDbResult.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi,true));
        }

        [HttpGet]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int listId, [FromRoute] int id)
        {
            if ((await oAuthDataApiService.Get<TaskResult>(nameof(ToDo), id)).Data is {} taskResult && taskResult.ListId == listId)
            {
                await oAuthDataApiService.Delete(nameof(ToDo), id);
            }
            
            return RedirectToAction("Index", new { listId });
        }
    }
}