namespace WorldBook.ViewModel
{
    public class PaginatedFeedbackViewModel
    {
        public IEnumerable<UserFeedbackViewModel> Feedbacks { get; set; } = new List<UserFeedbackViewModel>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 6; // 6 items per page
        public string CurrentFilter { get; set; } = "all";
        public int? CurrentOrderId { get; set; }
        public int TotalItems { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
