using StellarDsClient.Dto.Data.Request;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Ui.Mvc.Providers;
using System.Collections.Generic;
using StellarDsClient.Ui.Mvc.Models.EntityModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class DataApiServiceExtensions
    {
        internal static async Task<StellarDsResult<ListResult>> GetLastUpdatedList(this DataApiService<OAuthTokenProvider> dataApiService)
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

            var listStellarDsResult = await dataApiService.Find<ListResult>("list", listIndexFilter.GetQuery() + pagination.GetQuery());
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

            var taskStellarDsResult = await dataApiService.Find<TaskResult>("task", taskIndexFilter.GetQuery() + pagination.GetQuery());

            if (taskStellarDsResult.Data?.FirstOrDefault() is not { } taskResult)
            {
                return new StellarDsResult<ListResult>
                {
                    Messages = taskStellarDsResult.Messages,
                };
            }
            
            if (taskResult.Updated > listResult.Updated && taskResult.ListId != listResult.Id)
            {
                return await dataApiService.Get<ListResult>("list", taskResult.ListId);
            }

            return new StellarDsResult<ListResult>
            {
                Data = listResult
            };
        }

        internal static async Task DeleteListWithTasks(this DataApiService<OAuthTokenProvider> dataApiService, int id)
        {
            if ((await dataApiService.Find<TaskResult>("task", $"&whereQuery=ListId;equal;{id}")).Data is not { } taskResults)
            {
                return;  //todo error? do not allow to delete the list without deleting tasks possibly associated with it
            }

            await dataApiService.Delete("task", taskResults.Select(taskResult => taskResult.Id.ToString()).ToArray());

            await dataApiService.Delete("list", id);
        }

        internal static async Task<int> CreateListWithBlob(this DataApiService<OAuthTokenProvider> dataApiService, ListFormModel listFormModel, string ownerId, string ownerName)
        {
            if ((await dataApiService.Create<CreateListRequest, ListResult>("list", listFormModel.ToCreateListRequest(ownerId, ownerName))).Data is not { } listResult)
            {
                return 0;
            };

            if (listFormModel.ImageUpload is null)
            {
                return listResult.Id;
            }

            using var multipartFormDataContent = new MultipartFormDataContent().AddFormFile(listFormModel.ImageUpload);

            await dataApiService.UploadFileToApi("list", "Image", listResult.Id, multipartFormDataContent);

            return listResult.Id;
        }

        internal static async Task UpdateListWithBlob(this DataApiService<OAuthTokenProvider> dataApiService, int listId, ListFormModel listFormModel)
        {
            if ((await dataApiService.Put<PutListRequest, ListResult>("list", listId, listFormModel.ToPutListRequest())).Data is null)
            {
                return;
            };

            if (listFormModel.ImageUpload is null)
            {
                return;
            }

            using var multipartFormDataContent = new MultipartFormDataContent().AddFormFile(listFormModel.ImageUpload);

            await dataApiService.UploadFileToApi("list", "Image",listId, multipartFormDataContent);
        }

        internal static async Task<StellarDsResult<ListEntityModel>> GetListWithTasks(this DataApiService<ReadonlyAccessTokenProvider> dataApiService, int listId, Pagination pagination, TaskIndexFilter taskIndexFilter)
        { 
            var stellarDsListResult = await dataApiService.Get<ListResult>("list", listId);
            if (stellarDsListResult.Data is not { } listResult)
            {
                return new StellarDsResult<ListEntityModel>
                {
                    Messages = stellarDsListResult.Messages
                };
            }
            
            taskIndexFilter ??= new TaskIndexFilter();
            taskIndexFilter.ListId = listId;

            var stellarDsTaskResult = await dataApiService.Find<TaskResult>("task", pagination.GetQuery() + taskIndexFilter.GetQuery());
            if (stellarDsTaskResult.Data is not { } taskResults)
            {
                return new StellarDsResult<ListEntityModel>
                {
                    Messages = [.. stellarDsListResult.Messages, .. stellarDsTaskResult.Messages]
                };
            }
            
            var listTask = await listResult.ToListEntityModel(dataApiService.DownloadBlobFromApi);
            listTask.TaskResults = taskResults;

            return new StellarDsResult<ListEntityModel>
            {
                Data = listTask
            };
        }
    }
}