namespace WorldBook.ViewModel
{
    public class BookFeedbackViewModel
    {
        public int FeedbackId { get; set; }
        public string UserName { get; set; }
        public int? Star { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Reply { get; set; }
        public string? ReplyAccountName { get; set; }
        public DateTime? ReplyDate { get; set; }
        public int? OrderId { get; set; }
    }

    public class BookDetailWithFeedbackViewModel : BookDetailViewModel
    {
        public IEnumerable<BookFeedbackViewModel> Feedbacks { get; set; } = new List<BookFeedbackViewModel>();
        public RatingStatisticsViewModel RatingStatistics { get; set; } = new();
    }
}
