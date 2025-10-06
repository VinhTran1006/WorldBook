using System;
using System.ComponentModel.DataAnnotations;

namespace WorldBook.ViewModels
{
    public class UserEditViewModel
    {
        [Required] public int UserId { get; set; }
        [Required] public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        public string Phone { get; set; }

        // optional password change
        [MinLength(6)]
        public string NewPassword { get; set; }

        public bool IsActive { get; set; }
    }
}
