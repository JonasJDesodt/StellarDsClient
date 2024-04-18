using StellarDsClient.Models.Result;
using StellarDsClient.Ui.Mvc.Models.FormModels;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class TaskResultExtensions
    {
        public static IList<TaskFormModel> ToTaskFormModels(this IList<TaskResult> taskResults)
        {
            return taskResults.Select(tr => tr.ToTaskFormModel()).ToList();
        }

        public static TaskFormModel ToTaskFormModel(this TaskResult taskResult)
        {
            return new TaskFormModel
            {
                Finished = taskResult.Done,
                Title = taskResult.Title,
                Id = taskResult.Id,
            };
        }
    }
}
