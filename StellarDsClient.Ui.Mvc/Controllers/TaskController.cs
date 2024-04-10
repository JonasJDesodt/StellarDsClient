using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Dto.Data.Request;
using StellarDsClient.Dto.Data.Result;
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
    [Route("tasks/{listId:int}")]
    [ProvideOAuthBaseAddress]
    public class TaskController(DataApiService<ReadonlyAccessTokenProvider> readOnlyDataApiService, DataApiService<OAuthTokenProvider> oAuthDataApiService) : Controller
    {
        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index([FromRoute] int listId, [FromQuery] TaskIndexFilter? taskIndexFilter, [FromQuery] Pagination? pagination)
        {
            pagination ??= new Pagination();

            var stellarDsListResult = await readOnlyDataApiService.Get<ListResult>("list", listId);

            taskIndexFilter ??= new TaskIndexFilter();
            taskIndexFilter.ListId = listId;

            var stellarDsTaskResult = await readOnlyDataApiService.Find<TaskResult>("task", pagination.GetQuery() + taskIndexFilter.GetQuery());

            return View(await stellarDsTaskResult.ToTaskIndexViewModel(stellarDsListResult, readOnlyDataApiService.DownloadBlobFromApi, taskIndexFilter, pagination));
        }

        [HttpGet]
        [Route("create")]
        public async Task<IActionResult> Create([FromRoute] int listId)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>("list", listId);

            return View(await stellarDsListResult.ToTaskCreateEditViewModel(oAuthDataApiService.DownloadBlobFromApi));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<IActionResult> Create([FromRoute] int listId, [FromForm] TaskFormModel taskFormModel)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>("list", listId);

            if (!ModelState.IsValid)
            {
                return View(await taskFormModel.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));
            }

            await oAuthDataApiService.Create<CreateTaskRequest, TaskResult>("task", taskFormModel.ToCreateRequest(listId));
            //todo: error if no success?

            return RedirectToAction("Index", new { listId });
        }

        [HttpGet]
        [Route("edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] int listId, [FromRoute] int id)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>("list", listId);

            var stellarDsTaskResult = await oAuthDataApiService.Get<TaskResult>("task", id);

            return View(await stellarDsTaskResult.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] int listId, [FromRoute] int id, [FromForm] TaskFormModel taskFormModel)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>("list", listId);

            if (!ModelState.IsValid)
            {
                return View(await taskFormModel.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi));

            }

            await oAuthDataApiService.Put<PutTaskRequest, TaskResult>("task", id, taskFormModel.ToPutRequest());
            //todo: error if no success?

            return RedirectToAction("Index", new { listId });
        }

        [HttpGet]
        [Route("delete-request/{id:int}")]
        public async Task<IActionResult> DeleteRequest([FromRoute] int listId, [FromRoute] int id)
        {
            var stellarDsListResult = await oAuthDataApiService.Get<ListResult>("list", listId);

            if ((await oAuthDataApiService.Get<TaskResult>("task", id)) is not { } stellarDbResult)
            {
                return RedirectToAction("Index", "Task", new { listId }); // todo: error page
            }

            return View("Edit", await stellarDbResult.ToTaskCreateEditViewModel(stellarDsListResult, oAuthDataApiService.DownloadBlobFromApi,true));
        }

        [HttpGet]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int listId, [FromRoute] int id)
        {
            if ((await oAuthDataApiService.Get<TaskResult>("task", id)).Data is {} taskResult && taskResult.ListId == listId)
            {
                await oAuthDataApiService.Delete("task", id);
            }
            
            return RedirectToAction("Index", new { listId });
        }
    }
}