using System;
using System.ComponentModel.DataAnnotations;

namespace WorldBook.ViewModels
{
    public class UserCreateViewModel
    {
        [Required] public string Username { get; set; }
        [Required] public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        public string Phone { get; set; }

        [Required, MinLength(6)] public string Password { get; set; }
        [Compare("Password")] public string ConfirmPassword { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
