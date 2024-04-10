using StellarDsClient.Dto.Data.Request;
using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Delegates;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Models.ViewModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class TaskFormModelExtensions
    {
        public static CreateTaskRequest ToCreateRequest(this TaskFormModel taskFormModel, int listId)
        {
            var now = DateTime.UtcNow;

            return new CreateTaskRequest
            {
                Done = taskFormModel.Finished,
                ListId = listId,
                Title = taskFormModel.Title,
                Created = now,
                Updated = now
            };
        }

        public static PutTaskRequest ToPutRequest(this TaskFormModel taskFormModel)
        {
            return new PutTaskRequest
            {
                Done = taskFormModel.Finished,
                Title = taskFormModel.Title,
                Updated = DateTime.UtcNow
            };
        }

        public static async Task<TaskCreateEditViewModel> ToTaskCreateEditViewModel(this TaskFormModel taskFormModel, StellarDsResult<ListResult> stellarDsListResult, DownloadBlobFromApi downloadBlobFromApi)
        {
            if (stellarDsListResult.Data is not { } listResult)
            {
                return new TaskCreateEditViewModel
                {
                    ErrorMessages = stellarDsListResult.Messages
                };
            }

            return new TaskCreateEditViewModel
            {
                ListEntity = await listResult.ToListEntityModel(downloadBlobFromApi),
                TaskFormModel = taskFormModel
            };
        }
    }
}