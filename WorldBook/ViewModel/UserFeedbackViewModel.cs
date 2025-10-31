namespace WorldBook.ViewModel
{
    public class UserFeedbackViewModel
    {
        public int FeedbackId { get; set; }
        public int? UserId { get; set; }
        public int? BookId { get; set; }
        public int? OrderId { get; set; }
        public int? Star { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? BookName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
