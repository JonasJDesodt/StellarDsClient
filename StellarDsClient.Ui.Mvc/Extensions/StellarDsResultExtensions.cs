using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Delegates;
using StellarDsClient.Ui.Mvc.Models.UiModels;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.PartialModels;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.ViewModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class StellarDsResultExtensions
    {
        public static async Task<ListIndexViewModel> ToListIndexViewModel(this StellarDsResult<IList<ListResult>> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi, ListIndexFilter? filter, Pagination pagination)
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
                Lists = await Task.WhenAll(listResults.Select(async l => await l.ToListEntityModel(downloadBlobFromApi)).ToList())
            };
        }

        public static async Task<ListCreateEditViewModel> ToListCreateEditViewModel(this StellarDsResult<ListResult> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi, bool hasDeleteRequest = false)
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
                    CurrentImage = listResult.Image?.EndsWith("size=0") == false ? await downloadBlobFromApi("list", "image", listResult.Id) : null, //todo: using?, use tableSettings to get the tableId
                    HasDeleteRequest = hasDeleteRequest
                }
            };
        }

        public static async Task<TaskCreateEditViewModel> ToTaskCreateEditViewModel(this StellarDsResult<ListResult> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi)
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
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi)
            };
        }

        public static async Task<HomeViewModel> ToHomeViewModel(this StellarDsResult<ListResult> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi)
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
                List = await stellarDsResult.Data.ToListEntityModel(downloadBlobFromApi),
            };
        }

        public static TaskIndexViewModel ToTaskIndexViewModel(this StellarDsResult<ListUiModel> stellarDsResult, Pagination pagination, TaskIndexFilter taskIndexFilter)
        {
            var paginationPartialModel = pagination.ToPaginationPartialModel(stellarDsResult);

            if (stellarDsResult.Data is not { } listEntity)
            {
                return new TaskIndexViewModel
                {
                    ErrorMessages = stellarDsResult.Messages
                };
            }

            return new TaskIndexViewModel
            {
                PaginationPartialModel = paginationPartialModel,
                List = listEntity,
                TaskIndexFilter = taskIndexFilter
            };
        }

        public static async Task<TaskCreateEditViewModel> ToTaskCreateEditViewModel(this StellarDsResult<TaskResult> stellarDsTaskResult, StellarDsResult<ListResult> stellarDsListResult, DownloadBlobFromApi downloadBlobFromApi, bool hasDeleteRequest = false)
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
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi),
                TaskFormModel = taskFormModel
            };
        }

    }
}