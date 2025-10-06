using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Password { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Feedback> FeedbackReplyAccounts { get; set; } = new List<Feedback>();

    public virtual ICollection<Feedback> FeedbackUsers { get; set; } = new List<Feedback>();

    public virtual ICollection<ImportStock> ImportStocks { get; set; } = new List<ImportStock>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
