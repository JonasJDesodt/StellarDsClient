using StellarDsClient.Models.Result;
using StellarDsClient.Ui.Mvc.Models.FormModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ToDoResultExtensions
    {
        public static IList<ToDoFormModel> ToToDoFormModels(this IList<TaskResult> taskResults)
        {
            return taskResults.Select(tr => tr.ToToDoFormModel()).ToList();
        }

        public static ToDoFormModel ToToDoFormModel(this TaskResult taskResult)
        {
            return new ToDoFormModel
            {
                Finished = taskResult.Done,
                Title = taskResult.Title,
                Id = taskResult.Id,
            };
        }
    }
}
