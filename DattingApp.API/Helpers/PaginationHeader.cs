namespace DattingApp.API.Helpers
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public PaginationHeader(int cPage, int itemsPerPage, int totalItems, int totalPages)
        {
            this.CurrentPage = cPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalPages = totalPages;
            this.TotalItems = totalItems;
        }
    }
}