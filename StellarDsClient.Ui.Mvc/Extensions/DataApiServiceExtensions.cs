﻿using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Ui.Mvc.Providers;
using System.Collections.Generic;
using System.Reflection;
using StellarDsClient.Ui.Mvc.Models.UiModels;
using StellarDsClient.Models.Request;
using StellarDsClient.Models.Result;
using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Models.Mappers;
using System.Web;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class DataApiServiceExtensions
    {
        internal static async Task<StellarDsResult<ListResult>> GetLastUpdatedList(this DataApiService<OAuthAccessTokenProvider> dataApiService)
        {
            var pagination = new Pagination
            {
                Page = 1,
                PageSize = 1
            };

            //todo: use join?

            var listIndexFilter = new ListIndexFilter
            {
                Sort = "updated",
                SortAscending = false
            };

            var listStellarDsResult = await dataApiService.Find<ListResult>(nameof(List), listIndexFilter.GetQuery() + pagination.GetQuery());

            if (listStellarDsResult.Data?.FirstOrDefault() is not { } listResult)
            {
                return new StellarDsResult<ListResult>
                {
                    Messages = listStellarDsResult.Messages,
                };
            }


            var taskIndexFilter = new TaskIndexFilter
            {
                Sort = "updated",
                SortAscending = false
            };

            var taskStellarDsResult = await dataApiService.Find<ToDoResult>(nameof(ToDo), taskIndexFilter.GetQuery() + pagination.GetQuery());

            if (taskStellarDsResult.Data?.FirstOrDefault() is not { } taskResult)
            {
                return new StellarDsResult<ListResult>
                {
                    Data = listResult,
                };
            }
            
            if (taskResult.Updated > listResult.Updated && taskResult.ListId != listResult.Id)
            {
                return new StellarDsResult<ListResult>
                {
                    Data = listStellarDsResult.Data.FirstOrDefault(l => l.Id == taskResult.ListId),
                };
            }

            return new StellarDsResult<ListResult>
            {
                Data = listResult
            };
        }

        internal static async Task DeleteListWithTasks(this DataApiService<OAuthAccessTokenProvider> dataApiService, int id)
        {
            if ((await dataApiService.Find<ToDoResult>(nameof(ToDo), $"&whereQuery={HttpUtility.UrlEncode($"{nameof(ToDo.ListId)};equal;{id}")}")).Data is not { } taskResults)
            {
                return;  //todo error? do not allow to delete the list without deleting tasks possibly associated with it
            }

            await dataApiService.Delete(nameof(ToDo), taskResults.Select(taskResult => taskResult.Id.ToString()).ToArray());

            await dataApiService.Delete(nameof(List), id);
        }

        internal static async Task<int> CreateListWithBlob(this DataApiService<OAuthAccessTokenProvider> dataApiService, ListFormModel listFormModel, string ownerId, string ownerName)
        {
            if ((await dataApiService.Create<CreateListRequest, ListResult>(nameof(List), listFormModel.ToCreateListRequest(ownerId, ownerName))).Data is not { } listResult)
            {
                return 0;
            };

            if (listFormModel.ImageUpload is null)
            {
                return listResult.Id;
            }

            using var multipartFormDataContent = new MultipartFormDataContent().AddFormFile(listFormModel.ImageUpload);

            await dataApiService.UploadFileToApi(nameof(List), nameof(List.Image), listResult.Id, multipartFormDataContent);

            return listResult.Id;
        }

        internal static async Task UpdateListWithBlob(this DataApiService<OAuthAccessTokenProvider> dataApiService, int listId, ListFormModel listFormModel)
        {
            if ((await dataApiService.Put<PutListRequest, ListResult>(nameof(List), listId, listFormModel.ToPutListRequest())).Data is null)
            {
                return;
            };

            if (listFormModel.ImageUpload is null)
            {
                return;
            }

            using var multipartFormDataContent = new MultipartFormDataContent().AddFormFile(listFormModel.ImageUpload);

            await dataApiService.UploadFileToApi(nameof(List), nameof(List.Image),listId, multipartFormDataContent);
        }

        internal static async Task<StellarDsResult<ListUiModel>> GetListWithTasks(this DataApiService<ReadonlyAccessTokenProvider> dataApiService, int listId, Pagination pagination, TaskIndexFilter taskIndexFilter)
        { 
            var stellarDsListResult = await dataApiService.Get<ListResult>(nameof(List), listId);
            if (stellarDsListResult.Data is not { } listResult)
            {
                return new StellarDsResult<ListUiModel>
                {
                    Messages = stellarDsListResult.Messages,
                };
            }
            
            taskIndexFilter ??= new TaskIndexFilter();
            taskIndexFilter.ListId = listId;

            var stellarDsTaskResult = await dataApiService.Find<ToDoResult>(nameof(ToDo), pagination.GetQuery() + taskIndexFilter.GetQuery());
            if (stellarDsTaskResult.Data is not { } taskResults)
            {
                return new StellarDsResult<ListUiModel>
                {
                    Messages = [.. stellarDsListResult.Messages, .. stellarDsTaskResult.Messages]
                };
            }
            
            var listTask = await listResult.ToListEntityModel(dataApiService.DownloadBlobFromApi);
            listTask.TaskResults = taskResults; //todo
            listTask.TotalTaskResults = stellarDsTaskResult.Count; //todo

            return new StellarDsResult<ListUiModel>
            {
                Data = listTask
            };
        }
    }
}