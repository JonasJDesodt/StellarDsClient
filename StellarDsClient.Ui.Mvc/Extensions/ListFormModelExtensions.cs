using StellarDsClient.Dto.Data.Request;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Ui.Mvc.Delegates;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.ViewModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ListFormModelExtensions
    {
        public static CreateListRequest ToCreateListRequest(this ListFormModel listFormModel, string ownerId, string ownerName)
        {
            return new CreateListRequest
            {
                Created = DateTime.UtcNow,
                Title = listFormModel.Title,
                OwnerId = ownerId,
                OwnerName = ownerName,
                Deadline = listFormModel.Deadline
            };
        }

        public static PutListRequest ToPutListRequest(this ListFormModel listFormModel)
        {
            return new PutListRequest
            {
                Title = listFormModel.Title,
                Deadline = listFormModel.Deadline
            };
        }

        public static ListCreateEditViewModel ToListCreateEditViewModel(this ListFormModel listFormModel)
        {
            return new ListCreateEditViewModel
            {
                ListFormModel = listFormModel
            };
        }
    }
}