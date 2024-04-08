using System.Web;
using StellarDsClient.Ui.Mvc.Models.Filters;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class ListIndexFilterExtensions
    {
        public static string GetQuery(this ListIndexFilter? listIndexFilter)
        {
            if (listIndexFilter is null || (string.IsNullOrWhiteSpace(listIndexFilter.Title) && string.IsNullOrWhiteSpace(listIndexFilter.Owner) && listIndexFilter.CreatedStart is null && listIndexFilter.CreatedEnd is null && listIndexFilter.Sort is null))
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

            //todo deadline

            return query + HttpUtility.UrlEncode(string.Join("&", queries)) + $"&sortQuery={listIndexFilter.Sort ?? "created"};asc";
        }

        public static int GetActiveCount(this ListIndexFilter filter)
        {
            var count = 0;

            if (filter.Title is not null) count++;
            if (filter.DeadlineStart is not null) count++;
            if (filter.CreatedEnd is not null) count++;
            if (filter.CreatedStart is not null) count++;
            if (filter.Owner is not null) count++;
            if (filter.Sort is not null && filter.Sort != "created") count++;

            return count;
        }
    }
}