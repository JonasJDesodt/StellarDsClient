namespace StellarDsClient.Ui.Mvc.Models.Filters
{
    public class ListIndexFilter
    {
        public string? Title { get; set; }

        public string? Owner { get; set; }

        public DateTime? CreatedStart { get; set; }

        public DateTime? CreatedEnd { get; set; }

        public DateTime? DeadlineStart { get; set;}

        public DateTime? DeadlineEnd { get; set; }

        public string? Sort { get; set; }

        public bool? SortAscending { get; set; }

        public bool? Scoped { get; set; }
    }
}