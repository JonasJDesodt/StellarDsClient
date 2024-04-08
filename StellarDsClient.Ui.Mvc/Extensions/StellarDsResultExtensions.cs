using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Delegates;
using StellarDsClient.Ui.Mvc.Models.EntityModels;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.PartialModels;
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
                ListEntities = await Task.WhenAll(listResults.Select(async l => await l.ToListEntityModel(downloadBlobFromApi)).ToList())
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
                    CurrentImage = listResult.Image?.EndsWith("size=0") == false ? await downloadBlobFromApi(115, "image", listResult.Id) : null, //todo: using?, use tableSettings to get the tableId
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
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi),
            };
        }


        public static async Task<TaskIndexViewModel> ToTaskIndexViewModel(this StellarDsResult<IList<TaskResult>> stellarDsTaskResult, StellarDsResult<ListResult> stellarDsListResult, DownloadBlobFromApi downloadBlobFromApi, TaskIndexFilter? taskIndexFilter, Pagination pagination)
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
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi),
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