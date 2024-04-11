using System.Web;
using StellarDsClient.Ui.Mvc.Models.Filters;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ListIndexFilterExtensions
    {
        public static string GetQuery(this ListIndexFilter? listIndexFilter)
        {
            //todo: use model for fields
            //todo: return if sort is created & sortAscending is true
            if (listIndexFilter is null || (string.IsNullOrWhiteSpace(listIndexFilter.Title) && string.IsNullOrWhiteSpace(listIndexFilter.Owner) && listIndexFilter.CreatedStart is null && listIndexFilter.CreatedEnd is null && listIndexFilter.DeadlineStart is null && listIndexFilter.DeadlineEnd is null && listIndexFilter.Sort is null && listIndexFilter.SortAscending is null))
            {
                return string.Empty;
            }

            var queries = new List<string>();

            const string query = "&whereQuery=";

            if (!string.IsNullOrWhiteSpace(listIndexFilter.Title))
            {
                queries.Add($"Title;like;%{listIndexFilter.Title}%");
            }

            if (!string.IsNullOrWhiteSpace(listIndexFilter.Owner))
            {
                queries.Add($"OwnerName;like;%{listIndexFilter.Owner}%");
            }

            if (listIndexFilter.CreatedStart is { } createdStart)
            {

                queries.Add($"Created;largerThan;{createdStart:O}|Created;equal;{createdStart:O}");
            }

            if (listIndexFilter.CreatedEnd is { } createdEnd)
            {
                queries.Add($"Created;smallerThan;{createdEnd:O}|Created;equal;{createdEnd:O}");
            }

            if (listIndexFilter.DeadlineStart is { } deadlineStart)
            {

                queries.Add($"Deadline;largerThan;{deadlineStart:O}|Deadline;equal;{deadlineStart:O}");
            }

            if (listIndexFilter.DeadlineEnd is { } deadlineEnd)
            {
                queries.Add($"Deadline;smallerThan;{deadlineEnd:O}|Deadline;equal;{deadlineEnd:O}");
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