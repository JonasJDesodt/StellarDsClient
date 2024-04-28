using StellarDsClient.Models.Request;
using StellarDsClient.Models.Result;
using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Delegates;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.ViewModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ToDoFormModelExtensions
    {
        public static CreateTaskRequest ToCreateRequest(this ToDoFormModel toDoFormModel, int listId)
        {
            var now = DateTime.UtcNow;

            return new CreateTaskRequest
            {
                Done = toDoFormModel.Finished,
                ListId = listId,
                Title = toDoFormModel.Title,
                Created = now,
                Updated = now
            };
        }

        public static PutTaskRequest ToPutRequest(this ToDoFormModel toDoFormModel)
        {
            return new PutTaskRequest
            {
                Done = toDoFormModel.Finished,
                Title = toDoFormModel.Title,
                Updated = DateTime.UtcNow
            };
        }

        public static async Task<ToDoCreateEditViewModel> ToTaskCreateEditViewModel(this ToDoFormModel toDoFormMOdel, StellarDsResult<ListResult> stellarDsListResult, DownloadBlobFromApi downloadBlobFromApi)
        {
            if (stellarDsListResult.Data is not { } listResult)
            {
                return new ToDoCreateEditViewModel
                {
                    ErrorMessages = stellarDsListResult.Messages
                };
            }

            return new ToDoCreateEditViewModel
            {
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi),
                TaskFormModel = toDoFormMOdel
            };
        }
    }
}