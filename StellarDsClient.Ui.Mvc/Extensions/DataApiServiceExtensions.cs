using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.ViewModels;
using StellarDsClient.Ui.Mvc.Providers;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class DataApiServiceExtensions
    {
        internal static async Task<StellarDsResult<ListResult>> GetLastUpdatedList(this DataApiService<OAuthTokenProvider> dataApiService, int listTableId, int taskTableId)
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

            var listStellarDsResult = await dataApiService.Find<ListResult>(listTableId, listIndexFilter.GetQuery() + pagination.GetQuery());
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

            var taskStellarDsResult = await dataApiService.Find<TaskResult>(taskTableId, taskIndexFilter.GetQuery() + pagination.GetQuery());

            if (taskStellarDsResult.Data?.FirstOrDefault() is not { } taskResult)
            {
                return new StellarDsResult<ListResult>
                {
                    Messages = taskStellarDsResult.Messages,
                };
            }
            
            if (taskResult.Updated > listResult.Updated && taskResult.ListId != listResult.Id)
            {
                return await dataApiService.Get<ListResult>(listTableId, taskResult.ListId);
            }

            return new StellarDsResult<ListResult>
            {
                Data = listResult
            };
        }

    }
}