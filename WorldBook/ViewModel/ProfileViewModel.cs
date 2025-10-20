using System.ComponentModel.DataAnnotations;

namespace WorldBook.ViewModel
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-ZÀ-ỹ\s]+$", ErrorMessage = "Name must not contain special characters.")]
        public string Name { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date of birth is required.")]
        [CustomValidation(typeof(ProfileViewModel), nameof(ValidateDateOfBirth))]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please select your gender.")]
        public string? Gender { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9À-ỹ\s,.\-\/]*$", ErrorMessage = "Address contains invalid characters.")]
        public string? Address { get; set; }

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must contain exactly 10 digits.")]
        public string? Phone { get; set; }

        // ✅ Custom rule: DOB phải nhỏ hơn ngày hiện tại
        public static ValidationResult? ValidateDateOfBirth(DateTime? dob, ValidationContext context)
        {
            if (dob == null)
                return new ValidationResult("Date of birth is required.");

            if (dob >= DateTime.Today)
                return new ValidationResult("Date of birth must be before today.");

            return ValidationResult.Success;
        }
    }
}
