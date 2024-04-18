using System.Web;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Ui.Mvc.Models.Filters;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ListIndexFilterExtensions
    {
        public static string GetQuery(this ListIndexFilter? listIndexFilter)
        {
            //todo: return if sort is created & sortAscending is true
            if (listIndexFilter is null || (string.IsNullOrWhiteSpace(listIndexFilter.Title) && string.IsNullOrWhiteSpace(listIndexFilter.Owner) && listIndexFilter.CreatedStart is null && listIndexFilter.CreatedEnd is null && listIndexFilter.DeadlineStart is null && listIndexFilter.DeadlineEnd is null && listIndexFilter.Sort is null && listIndexFilter.SortAscending is null))
            {
                return string.Empty;
            }

            var queries = new List<string>();

            const string query = "&whereQuery=";

            if (!string.IsNullOrWhiteSpace(listIndexFilter.Title))
            {
                queries.Add($"{nameof(List.Title)};like;%{listIndexFilter.Title}%");
            }

            if (!string.IsNullOrWhiteSpace(listIndexFilter.Owner))
            {
                queries.Add($"{nameof(List.OwnerName)};like;%{listIndexFilter.Owner}%");
            }

            if (listIndexFilter.CreatedStart is { } createdStart)
            {

                queries.Add($"{nameof(List.Created)};largerThan;{createdStart:O}|{nameof(List.Created)};equal;{createdStart:O}");
            }

            if (listIndexFilter.CreatedEnd is { } createdEnd)
            {
                queries.Add($"{nameof(List.Created)};smallerThan;{createdEnd:O}|{nameof(List.Created)};equal;{createdEnd:O}");
            }

            if (listIndexFilter.DeadlineStart is { } deadlineStart)
            {

                queries.Add($"{nameof(List.Deadline)};largerThan;{deadlineStart:O}|{nameof(List.Deadline)};equal;{deadlineStart:O}");
            }

            if (listIndexFilter.DeadlineEnd is { } deadlineEnd)
            {
                queries.Add($"{nameof(List.Deadline)};smallerThan;{deadlineEnd:O}|{nameof(List.Deadline)};equal;{deadlineEnd:O}");
            }

            return query + HttpUtility.UrlEncode(string.Join("&", queries)) + $"&sortQuery={listIndexFilter.Sort ?? "created"};{(listIndexFilter.SortAscending is true or null ? "asc" : "desc")}";
        }

        public static int GetActiveCount(this ListIndexFilter filter)
        {
            var count = 0;

            if (filter.Title is not null) count++;
            if (filter.CreatedEnd is not null) count++;
            if (filter.CreatedStart is not null) count++;
            if (filter.DeadlineStart is not null) count++;
            if (filter.DeadlineEnd is not null) count++;
            if (filter.Owner is not null) count++;
            if (filter.Sort is not null && filter.Sort != "created") count++;
            if (filter.SortAscending is not null && filter.SortAscending == false) count++;

            return count;
        }
    }
}