using System.Collections.Generic;
using System.Reflection;
using System.Web;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Ui.Mvc.Models.Filters;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ToDoIndexFilterExtensions
    {
        public static string GetQuery(this TaskIndexFilter? taskIndexFilter)
        {
            //todo: use StringBuilder or placeholders?

            if (taskIndexFilter is null)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(taskIndexFilter.Title) && taskIndexFilter.CreatedStart is null && taskIndexFilter.CreatedEnd is null && taskIndexFilter.Sort is null && taskIndexFilter.ListId is null && taskIndexFilter.SortAscending is null)
            {
                return string.Empty;
            }

            var queries = new List<string>();

            const string query = "&whereQuery=";

            if (!string.IsNullOrWhiteSpace(taskIndexFilter.Title))
            {
                queries.Add($"{nameof(ToDo.Title)};like;%{taskIndexFilter.Title}%");
            }

            if (taskIndexFilter.CreatedStart is { } createdStart)
            {

                queries.Add($"{nameof(ToDo.Created)};largerThan;{createdStart:O}|{nameof(ToDo.Created)};equal;{createdStart:O}");
            }

            if (taskIndexFilter.CreatedEnd is { } createdEnd)
            {
                queries.Add($"{nameof(ToDo.Created)};smallerThan;{createdEnd:O}|{nameof(ToDo.Created)};equal;{createdEnd:O}");
            }

            if (taskIndexFilter.ListId is { } listId)
            {
                queries.Add($"{nameof(ToDo.ListId)};equal;{listId}");
            }

            return query + HttpUtility.UrlEncode(string.Join("&", queries)) + $"&sortQuery={taskIndexFilter.Sort ?? "created"};{(taskIndexFilter.SortAscending is true or null ? "asc" : "desc")}";
        }

        public static int GetActiveCount(this TaskIndexFilter filter)
        {
            var count = 0;

            if (filter.CreatedEnd is not null) count++;
            if (filter.CreatedStart is not null) count++;
            if (filter.Title is not null) count++;
            if (filter.Sort is not null && filter.Sort != "created") count++;
            if (filter.SortAscending is not null && filter.SortAscending == false) count++;

            return count;
        }
    }
}
