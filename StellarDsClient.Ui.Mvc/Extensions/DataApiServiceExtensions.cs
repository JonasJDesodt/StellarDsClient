using StellarDsClient.Dto.Data.Request;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Ui.Mvc.Providers;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class DataApiServiceExtensions
    {
        internal static async Task<StellarDsResult<ListResult>> GetLastUpdatedList(this DataApiService<OAuthTokenProvider> dataApiService, TableSettings tableSettings)
        {
            var pagination = new Pagination
            {
                Page = 1,
                PageSize = 1
            };

            //todo: use join?

            var listIndexFilter = new ListIndexFilter
            {
                Sort = "updated"
            };

            var listStellarDsResult = await dataApiService.Find<ListResult>(tableSettings.ListTableId, listIndexFilter.GetQuery() + pagination.GetQuery());
            if (listStellarDsResult.Data?.FirstOrDefault() is not { } listResult)
            {
                return new StellarDsResult<ListResult>
                {
                    Messages = listStellarDsResult.Messages,
                };
            }


            var taskIndexFilter = new TaskIndexFilter
            {
                Sort = "updated"
            };

            var taskStellarDsResult = await dataApiService.Find<TaskResult>(tableSettings.TaskTableId, taskIndexFilter.GetQuery() + pagination.GetQuery());

            if (taskStellarDsResult.Data?.FirstOrDefault() is not { } taskResult)
            {
                return new StellarDsResult<ListResult>
                {
                    Messages = taskStellarDsResult.Messages,
                };
            }
            
            if (taskResult.Updated > listResult.Updated && taskResult.ListId != listResult.Id)
            {
                return await dataApiService.Get<ListResult>(tableSettings.ListTableId, taskResult.ListId);
            }

            return new StellarDsResult<ListResult>
            {
                Data = listResult
            };
        }

        internal static async Task DeleteListWithTasks(this DataApiService<OAuthTokenProvider> dataApiService, int id, TableSettings tableSettings)
        {
            if ((await dataApiService.Find<TaskResult>(tableSettings.TaskTableId, $"&whereQuery=ListId;equal;{id}")).Data is not { } taskResults)
            {
                return;  //todo error? do not allow to delete the list without deleting tasks possibly associated with it
            }

            await dataApiService.Delete(tableSettings.TaskTableId, taskResults.Select(taskResult => taskResult.Id.ToString()).ToArray());

            await dataApiService.Delete(tableSettings.ListTableId, id);
        }

        internal static async Task<int> CreateWithBlob(this DataApiService<OAuthTokenProvider> dataApiService, ListFormModel listFormModel, TableSettings tableSettings, string ownerId, string ownerName)
        {
            if ((await dataApiService.Create<CreateListRequest, ListResult>(tableSettings.ListTableId, listFormModel.ToCreateListRequest(ownerId, ownerName))).Data is not { } listResult)
            {
                return 0;
            };

            if (listFormModel.ImageUpload is null)
            {
                return listResult.Id;
            }

            using var multipartFormDataContent = new MultipartFormDataContent().AddFormFile(listFormModel.ImageUpload);

            await dataApiService.UploadFileToApi(tableSettings.ListTableId, "Image", listResult.Id, multipartFormDataContent);

            return listResult.Id;
        }

        internal static async Task UpdateWithBlob(this DataApiService<OAuthTokenProvider> dataApiService, int listId, ListFormModel listFormModel, TableSettings tableSettings)
        {
            if ((await dataApiService.Put<PutListRequest, ListResult>(tableSettings.ListTableId, listId, listFormModel.ToPutListRequest())).Data is null)
            {
                return;
            };

            if (listFormModel.ImageUpload is null)
            {
                return;
            }

            using var multipartFormDataContent = new MultipartFormDataContent().AddFormFile(listFormModel.ImageUpload);

            await dataApiService.UploadFileToApi(tableSettings.ListTableId, "Image",listId, multipartFormDataContent);
        }
    }
}