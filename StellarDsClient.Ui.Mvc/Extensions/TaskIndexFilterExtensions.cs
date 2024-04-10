using System.Collections.Generic;
using System.Web;
using StellarDsClient.Ui.Mvc.Models.Filters;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class TaskIndexFilterExtensions
    {
        public static string GetQuery(this TaskIndexFilter? taskIndexFilter)
        {
            //todo: use StringBuilder or placeholders?

            if (taskIndexFilter is null)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(taskIndexFilter.Title) && taskIndexFilter.CreatedStart is null && taskIndexFilter.CreatedEnd is null && taskIndexFilter.Sort is null && taskIndexFilter.ListId is null)
            {
                return string.Empty;
            }

            var queries = new List<string>();

            const string query = "&whereQuery=";

            if (!string.IsNullOrWhiteSpace(taskIndexFilter.Title))
            {
                queries.Add($"Title;like;%{taskIndexFilter.Title}%");
            }

            if (taskIndexFilter.CreatedStart is { } createdStart)
            {

                queries.Add($"Created;largerThan;{createdStart:O}|Created;equal;{createdStart:O}");
            }

            if (taskIndexFilter.CreatedEnd is { } createdEnd)
            {
                queries.Add($"Created;smallerThan;{createdEnd:O}|Created;equal;{createdEnd:O}");
            }

            if (taskIndexFilter.ListId is { } listId)
            {
                queries.Add($"ListId;equal;{listId}");
            }

            return query + HttpUtility.UrlEncode(string.Join("&", queries)) + $"&sortQuery={taskIndexFilter.Sort ?? "created"};desc";
            
            //return taskIndexFilter.Sort switch
            //{
            //    "done" => query + $"&sortQuery=done;desc",
            //    "title" => query + $"&sortQuery=title;asc",
            //    "updated" => query + $"&sortQuery=updated;desc",
            //    _ => query + $"&sortQuery=created;asc"
            //};
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
