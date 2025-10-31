namespace WorldBook.ViewModel
{
    public class FeedbackViewModel
    {
        public int FeedbackId { get; set; }
        public string? BookName { get; set; }
        public string? ImageUrl { get; set; }
        public string? UserName { get; set; }
        public int? Star { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
