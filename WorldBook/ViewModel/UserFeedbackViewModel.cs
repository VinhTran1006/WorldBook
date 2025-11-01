using System.ComponentModel.DataAnnotations;

namespace WorldBook.ViewModel
{
    public class UserFeedbackViewModel
    {
        public int FeedbackId { get; set; }
        public int? UserId { get; set; }
        public int? BookId { get; set; }
        public int? OrderId { get; set; }

        [Required(ErrorMessage = "Please select a rating")]
        public int? Star { get; set; }
        [StringLength(255, ErrorMessage = "Comment cannot exceed 255 characters")]
        public string? Comment { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? BookName { get; set; }
        public string? ImageUrl { get; set; }

        public string? Reply { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string? ReplyAccountName { get; set; }
    }
}
