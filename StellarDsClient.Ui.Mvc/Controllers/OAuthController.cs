using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Attributes;
using StellarDsClient.Sdk.Dto.Schema;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using System.Reflection;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    public class OAuthController(OAuthAccessTokenProvider oAuthAccessTokenProvider, SchemaApiService<OAuthAccessTokenProvider> schemaApiService, TableNames tableNames, TableSettings tableSettings) : Controller
    {
        private readonly OAuthApiService _oAuthApiService = oAuthAccessTokenProvider.OAuthApiService;

        [HttpGet]
        public IActionResult Index([FromQuery] string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;

            return View();
        }

        [HttpGet]
        public IActionResult SignIn([FromQuery] string returnUrl)
        {
            TempData["returnUrl"] = TempData["returnUrl"] ?? returnUrl;

            return Redirect($"{_oAuthApiService.OAuthBaseAddress}/oauth?client_id={_oAuthApiService.ClientId}&redirect_uri={_oAuthApiService.RedirectUri}&response_type=code");
        }

        [HttpGet]
        public async Task<IActionResult> OAuthCallback([FromQuery] string code)
        {
            var tokens = await _oAuthApiService.GetTokensAsync(code);

            await oAuthAccessTokenProvider.BrowserSignIn(tokens);

            var returnUrl = TempData["returnUrl"] as string;

            var stellarDsResult = await schemaApiService.FindTables();
            if (stellarDsResult?.Data is not { } tables)
            {
                return returnUrl == null ? RedirectToAction("Index", "Home") : LocalRedirect(returnUrl);
            }

            TableResult? listTableResult = null;
            TableResult? toDoTableResult = null;

            if (tables.Count == 0)
            {
                listTableResult = (await schemaApiService.CreateTable(typeof(List), tableNames.ListTableName))?.Data;

                toDoTableResult = (await schemaApiService.CreateTable(typeof(ToDo), tableNames.ToDoTableName))?.Data;

            }
            else
            {
                if (tables.FirstOrDefault(t => t.Name.Equals(tableNames.ListTableName)) is not { } existingListTable)
                {
                    listTableResult = (await schemaApiService.CreateTable(typeof(List), tableNames.ListTableName))?.Data;
                }
                else
                {
                    if ((await schemaApiService.GetFields(existingListTable.Id)).Data is { } existingListTableFields && existingListTable.IsValid(existingListTableFields, typeof(List)))
                    {
                        listTableResult = existingListTable;
                    }
                }

                if (tables.FirstOrDefault(t => t.Name.Equals(tableNames.ToDoTableName)) is not { } existingToDoTable)
                {
                    toDoTableResult = (await schemaApiService.CreateTable(typeof(ToDo), tableNames.ToDoTableName))?.Data;
                }
                else
                {
                    if ((await schemaApiService.GetFields(existingToDoTable.Id)).Data is { } existingToDoTableFields && existingToDoTable.IsValid(existingToDoTableFields, typeof(ToDo)))
                    {
                        toDoTableResult = existingToDoTable;
                    }
                }
            }

            if (listTableResult is not null && toDoTableResult is not null)
            {
                tableSettings[nameof(List)] = listTableResult.Id;
                tableSettings[nameof(ToDo)] = toDoTableResult.Id;
            }

            return returnUrl == null ? RedirectToAction("Index", "Home") : LocalRedirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut([FromQuery] string returnUrl)
        {
            await oAuthAccessTokenProvider.BrowserSignOut();

            return LocalRedirect(returnUrl);
        }


        private static bool IsTableValid(TableResult existingTable, IList<FieldResult> fieldResults, Type model)
        {
            var stellarDsTable = model.GetCustomAttribute<StellarDsTable>();

            if(stellarDsTable is null)
            {
                return false;
            }

            if(existingTable.IsMultitenant != stellarDsTable.IsMultiTenant)
            {
                return false;
            }

            if(existingTable.Description?.Equals(stellarDsTable.Description) is false)
            {
                return false;
            }

            foreach (var property in model.GetProperties())
            {
                var stellarDsType = property.GetCustomAttribute<StellarDsProperty>()?.Type;
                if (stellarDsType is null)
                {
                    return false;
                }

                if (!fieldResults.Any(f => f.Name.Equals(property.Name) && f.Type.Equals(stellarDsType)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}