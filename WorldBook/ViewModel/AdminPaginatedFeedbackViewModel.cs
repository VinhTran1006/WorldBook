namespace WorldBook.ViewModel
{
    public class AdminPaginatedFeedbackViewModel
    {
        public IEnumerable<FeedbackViewModel> Feedbacks { get; set; } = new List<FeedbackViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
