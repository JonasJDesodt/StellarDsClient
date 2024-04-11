namespace StellarDsClient.Ui.Mvc.Models.Filters
{
    public class TaskIndexFilter
    {
        public string? Title { get; set; }

        public DateTime? CreatedStart { get; set; }

        public DateTime? CreatedEnd { get; set; }

        public string? Sort { get; set; }

        public bool? SortAscending { get; set; }

        public int? ListId { get; set; }
    }
}
