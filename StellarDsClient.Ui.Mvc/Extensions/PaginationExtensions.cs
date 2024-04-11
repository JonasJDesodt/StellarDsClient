using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.PartialModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class PaginationExtensions
    {
        public static string GetQuery(this Pagination pagination)
        {
            return $"&offset={(pagination.Page - 1) * pagination.PageSize}&take={pagination.PageSize}";
        }

        //to do: TotalCount param instead of StellarDsResult
        public static PaginationPartialModel ToPaginationPartialModel<T>(this Pagination pagination, StellarDsResult<T> stellarDsResult) where T : class
        {
            return new PaginationPartialModel(pagination.Page, pagination.PageSize, stellarDsResult.Count);
        }
    }
}