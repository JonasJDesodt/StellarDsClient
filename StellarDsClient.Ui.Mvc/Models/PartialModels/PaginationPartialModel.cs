namespace StellarDsClient.Ui.Mvc.Models.PartialModels
{   
    public class PaginationPartialModel
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public PaginationPartialModel(int currentPage, int pageSize, int totalItems)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;

            var totalPages = (int)Math.Ceiling(totalItems / (decimal)PageSize);

            var startPage = currentPage - 5;
            var endPage = currentPage + 4;

            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }

            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
