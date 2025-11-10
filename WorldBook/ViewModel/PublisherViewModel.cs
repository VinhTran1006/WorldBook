using System.ComponentModel.DataAnnotations;

namespace WorldBook.ViewModel
{
    public class PublisherViewModel
    {
        public int PublisherId { get; set; }

        [Required(ErrorMessage = "Publisher name is required")]
        [Display(Name = "Publisher Name")]
        [StringLength(255, ErrorMessage = "Publisher name cannot exceed 255 characters")]
        [RegularExpression(@"^[A-Za-zÀ-ỹ\s]{3,}$", ErrorMessage = "Publisher name must be at least 3 letters and contain only alphabetic characters")]
        public string PublisherName { get; set; } = string.Empty;
    }
}
