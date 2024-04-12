using StellarDsClient.Builder.Library.Models;
using StellarDsClient.Dto.Data.Request;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Delegates;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.UiModels;
using StellarDsClient.Ui.Mvc.Models.ViewModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ListResultExtensions
    {
        public static async Task<ListFormModel> ToListFormModel(this ListResult listResult, DownloadBlobFromApi downloadBlobFromApi)
        {
            return new ListFormModel
            {
                Id = listResult.Id,
                Deadline = listResult.Deadline,
                Title = listResult.Title,
                CurrentImage = listResult.Image?.EndsWith("size=0") == false ? await downloadBlobFromApi(nameof(List), "Image", listResult.Id) : null //todo: using?, use tableSettings to get the tableId
            };
        }

        public static PutListRequest ToPutListRequest(this ListResult listResult)
        {
            return new PutListRequest
            {
                Deadline = listResult.Deadline,
                Title = listResult.Title,
                Updated = DateTime.UtcNow,
            };
        }

        public static async Task<ListUiModel> ToListEntityModel(this ListResult listResult, DownloadBlobFromApi downloadBlobFromApi)
        {
            return new ListUiModel
            {
                Created = listResult.Created,
                Title = listResult.Title,
                OwnerId = listResult.OwnerId,
                OwnerName = listResult.OwnerName,
                Deadline = listResult.Deadline,
                Id = listResult.Id,
                Image = listResult.Image?.EndsWith("size=0") == false ? await downloadBlobFromApi(nameof(List), "Image", listResult.Id) : null //todo: using?, use tableSettings to get the tableId
            };
        }
    }
}
