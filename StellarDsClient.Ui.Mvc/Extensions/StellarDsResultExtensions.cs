using StellarDsClient.Ui.Mvc.Delegates;
using StellarDsClient.Ui.Mvc.Models.UiModels;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.PartialModels;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Models.Result;
using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Models.Mappers;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class StellarDsResultExtensions
    {
        public static async Task<ListIndexViewModel> ToListIndexViewModel(this StellarDsResult<IList<ListResult>> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi, ListIndexFilter? filter, Pagination pagination)
        {
            var paginationPartialModel = pagination.ToPaginationPartialModel(stellarDsResult.Count);

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
                    CurrentImage = listResult.Image?.EndsWith("size=0") == false ? await downloadBlobFromApi(nameof(List), nameof(List.Image), listResult.Id) : null, //todo: using?
                    HasDeleteRequest = hasDeleteRequest
                }
            };
        }

        public static async Task<ToDoCreateEditViewModel> ToTaskCreateEditViewModel(this StellarDsResult<ListResult> stellarDsResult, DownloadBlobFromApi downloadBlobFromApi)
        {
            if (stellarDsResult.Data is not { } listResult)
            {
                return new ToDoCreateEditViewModel
                {
                    ErrorMessages = stellarDsResult.Messages
                };
            }

            return new ToDoCreateEditViewModel
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

        public static ToDoIndexViewModel ToTaskIndexViewModel(this StellarDsResult<ListUiModel> stellarDsResult, Pagination pagination, TaskIndexFilter taskIndexFilter)
        {
            var paginationPartialModel = pagination.ToPaginationPartialModel(stellarDsResult.Data?.TotalTaskResults ?? 0); //todo: handle null check better

            if (stellarDsResult.Data is not { } listEntity)
            {
                return new ToDoIndexViewModel
                {
                    ErrorMessages = stellarDsResult.Messages
                };
            }

            return new ToDoIndexViewModel
            {
                PaginationPartialModel = paginationPartialModel,
                List = listEntity,
                TaskIndexFilter = taskIndexFilter
            };
        }

        public static async Task<ToDoCreateEditViewModel> ToTaskCreateEditViewModel(this StellarDsResult<ToDoResult> stellarDsTaskResult, StellarDsResult<ListResult> stellarDsListResult, DownloadBlobFromApi downloadBlobFromApi, bool hasDeleteRequest = false)
        {
            if (stellarDsTaskResult.Data is not { } taskResult || stellarDsListResult.Data is not { } listResult)
            {
                return new ToDoCreateEditViewModel
                {
                    ErrorMessages = [.. stellarDsTaskResult.Messages, .. stellarDsListResult.Messages],
                };
            }

            var taskFormModel = taskResult.ToToDoFormModel();
            taskFormModel.HasDeleteRequest = hasDeleteRequest;

            return new ToDoCreateEditViewModel
            {
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi),
                TaskFormModel = taskFormModel
            };
        }
    }
}