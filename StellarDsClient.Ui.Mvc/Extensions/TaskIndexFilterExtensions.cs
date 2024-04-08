using System.Web;
using StellarDsClient.Ui.Mvc.Models.Filters;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class TaskIndexFilterExtensions
    {
        public static string GetQuery(this TaskIndexFilter? taskIndexFilter, int listId)
        {
            if (listId <= 0 && (taskIndexFilter is null || (string.IsNullOrWhiteSpace(taskIndexFilter.Title) && taskIndexFilter.CreatedStart is null && taskIndexFilter.CreatedEnd is null && taskIndexFilter.Sort is null)))
            {
                return string.Empty;
            }

            var query = "";
             
            if (!string.IsNullOrWhiteSpace(taskIndexFilter.Title))
            {
                query += '&' + $"Title;like;%{taskIndexFilter.Title}%";
            }

            if (taskIndexFilter.CreatedStart is { } createdStart)
            {

                query += '&' + $"Created;largerThan;{createdStart:O}|Created;equal;{createdStart:O}";
            }

            if (taskIndexFilter.CreatedEnd is { } createdEnd)
            {
                query += '&' + $"Created;smallerThan;{createdEnd:O}|Created;equal;{createdEnd:O}";
            }

            query = "&whereQuery=" + HttpUtility.UrlEncode($"ListId;equal;{listId}" + query);

            return taskIndexFilter.Sort switch
            {
                "done" => query + $"&sortQuery=done;desc",
                "title" => query + $"&sortQuery=title;asc",
                _ => query + $"&sortQuery=created;asc"
            };
        }

        public static int GetActiveCount(this TaskIndexFilter filter)
        {
            var count = 0;

            if (filter.CreatedEnd is not null) count++;
            if (filter.CreatedStart is not null) count++;
            if (filter.Title is not null) count++;
            if (filter.Sort is not null && filter.Sort != "created") count++;

            return count;
        }
    }
}
