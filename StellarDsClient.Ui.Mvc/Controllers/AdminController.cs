using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Services;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [Authorize]
    public class AdminController(SchemaApiService<OAuthAccessTokenProvider> schemaApiService, SqliteService sqliteService, TableSettings tableSettings) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetTables()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetTables([FromForm] ResetTablesFormModel resetTablesFormModel)
        {
            if (!ModelState.IsValid)
            {
                return View(resetTablesFormModel);
            }

            await CreateTables();

            return RedirectToAction("Index");
        }

        private async Task CreateTables()
        {
            var tablesStellarDsResult = await schemaApiService.FindTables();

            if (tablesStellarDsResult.IsSuccess is false || tablesStellarDsResult.Data is not { } tables)
            {
                return; //todo: errorHandling?
            }

            tables = [.. tables.OrderByDescending(t => t.Name.Length)];

            var toDoTableName = nameof(ToDo);
            if (tables.FirstOrDefault(t => t.Name.StartsWith(toDoTableName, StringComparison.InvariantCultureIgnoreCase))?.Name is { } existingToDoTableName)
            {
                toDoTableName = existingToDoTableName + "_1";
            }

            var listTableName = nameof(List);
            if (tables.FirstOrDefault(t => t.Name.StartsWith(listTableName, StringComparison.InvariantCultureIgnoreCase))?.Name is { } existingListTableName)
            {
                listTableName = existingListTableName + "_1";
            }

            var toDoTableStellarDsResult = await schemaApiService.CreateTable(typeof(ToDo), toDoTableName);
            if (toDoTableStellarDsResult.IsSuccess is false || toDoTableStellarDsResult.Data is not { } toDoTableMetaData)
            {

                return; //todo: errorHandling?
            }

            var listTableStellarDsResult = await schemaApiService.CreateTable(typeof(List), listTableName);
            if (listTableStellarDsResult.IsSuccess is false || listTableStellarDsResult.Data is not { } listTableMetaData)
            {
                await schemaApiService.DeleteTable(toDoTableMetaData.Id);

                return; //todo: errorHandling?
            }

            //todo: try/catch
            sqliteService.UpdateTableSettings(listTableMetaData.Id, toDoTableMetaData.Id);

            tableSettings[nameof(List)] = listTableMetaData.Id;
            tableSettings[nameof(ToDo)] = toDoTableMetaData.Id;
        }
    }
}