using System.ComponentModel.DataAnnotations;

namespace WorldBook.ViewModel
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [Display(Name = "Full Name")]
        public string Name { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }
    }
}
