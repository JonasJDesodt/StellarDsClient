using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Delegates;
using StellarDsClient.Ui.Mvc.Models.EntityModels;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.PartialModels;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.ViewModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class StellarDsResultExtensions
    {
        public static async Task<ListIndexViewModel> ToListIndexViewModel(this StellarDsResult<IList<ListResult>> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi, ListIndexFilter? filter, Pagination pagination, TableSettings tableSettings)
        {
            var paginationPartialModel = pagination.ToPaginationPartialModel(stellarDsResult);

            if (stellarDsResult.Data is not { } listResults)
            {
                return new ListIndexViewModel
                {
                    ErrorMessages = stellarDsResult.Messages
                };
            }

            return new ListIndexViewModel
            {
                PaginationPartialModel = paginationPartialModel,
                ListIndexFilter = filter,
                ListEntities = await Task.WhenAll(listResults.Select(async l => await l.ToListEntityModel(downloadBlobFromApi, tableSettings)).ToList())
            };
        }

        public static async Task<ListCreateEditViewModel> ToListCreateEditViewModel(this StellarDsResult<ListResult> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi, TableSettings tableSettings, bool hasDeleteRequest = false)
        {
            if (stellarDsResult.Data is not { } listResult)
            {
                return new ListCreateEditViewModel
                {
                    ErrorMessages = stellarDsResult.Messages
                };
            }

            return new ListCreateEditViewModel
            {
                ListFormModel = new ListFormModel //todo: toListFormModel
                {
                    Id = listResult.Id,
                    Deadline = listResult.Deadline,
                    Title = listResult.Title,
                    CurrentImage = listResult.Image?.EndsWith("size=0") == false ? await downloadBlobFromApi(tableSettings.ListTableId, "image", listResult.Id) : null, //todo: using?, use tableSettings to get the tableId
                    HasDeleteRequest = hasDeleteRequest
                }
            };
        }

        public static async Task<TaskCreateEditViewModel> ToTaskCreateEditViewModel(this StellarDsResult<ListResult> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi, TableSettings tableSettings)
        {
            if (stellarDsResult.Data is not { } listResult)
            {
                return new TaskCreateEditViewModel
                {
                    ErrorMessages = stellarDsResult.Messages
                };
            }

            return new TaskCreateEditViewModel
            {
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi, tableSettings),
            };
        }

        public static async Task<HomeViewModel> ToHomeViewModel(this StellarDsResult<ListResult> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi, TableSettings tableSettings)
        {
            if (stellarDsResult.Messages.Count > 0)
            {
                return new HomeViewModel
                {
                    ErrorMessages = stellarDsResult.Messages
                };
            }

            if (stellarDsResult.Data is not { } listResult)
            {
                return new HomeViewModel();
            }

            return new HomeViewModel
            {
                ListEntity = await stellarDsResult.Data.ToListEntityModel(downloadBlobFromApi, tableSettings),
            };
        }


        public static async Task<TaskIndexViewModel> ToTaskIndexViewModel(this StellarDsResult<IList<TaskResult>> stellarDsTaskResult, StellarDsResult<ListResult> stellarDsListResult, DownloadBlobFromApi downloadBlobFromApi, TaskIndexFilter? taskIndexFilter, Pagination pagination, TableSettings tableSettings)
        {
            var paginationPartialModel = pagination.ToPaginationPartialModel(stellarDsTaskResult);

            if (stellarDsTaskResult.Data is not { } taskResults || stellarDsListResult.Data is not { } listResult)
            {
                return new TaskIndexViewModel
                {
                    ErrorMessages = [.. stellarDsTaskResult.Messages, .. stellarDsListResult.Messages],
                };
            }

            return new TaskIndexViewModel
            {
                PaginationPartialModel = paginationPartialModel,
                TaskResults = taskResults,
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi, tableSettings),
                TaskIndexFilter = taskIndexFilter
            };
        }

        public static async Task<TaskCreateEditViewModel> ToTaskCreateEditViewModel(this StellarDsResult<TaskResult> stellarDsTaskResult, StellarDsResult<ListResult> stellarDsListResult, DownloadBlobFromApi downloadBlobFromApi, TableSettings tableSettings, bool hasDeleteRequest = false)
        {
            if (stellarDsTaskResult.Data is not { } taskResult || stellarDsListResult.Data is not { } listResult)
            {
                return new TaskCreateEditViewModel
                {
                    ErrorMessages = [.. stellarDsTaskResult.Messages, .. stellarDsListResult.Messages],
                };
            }

            var taskFormModel = taskResult.ToTaskFormModel();
            taskFormModel.HasDeleteRequest = hasDeleteRequest;

            return new TaskCreateEditViewModel
            {
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi, tableSettings),
                TaskFormModel = taskFormModel
            };
        }

    }
}