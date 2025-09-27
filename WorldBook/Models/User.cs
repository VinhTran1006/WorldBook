using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Password { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
