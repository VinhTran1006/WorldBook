using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WorldBook.Models;

public partial class WorldBookDbContext : DbContext
{
    public WorldBookDbContext(DbContextOptions<WorldBookDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<BookCategory> BookCategories { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<ImportStock> ImportStocks { get; set; }

    public virtual DbSet<ImportStockDetail> ImportStockDetails { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Author__70DAFC34FE6F42C1");

            entity.ToTable("Author");

            entity.Property(e => e.AuthorDescription)
                .HasMaxLength(2550)
                .IsUnicode(false);
            entity.Property(e => e.AuthorName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Book__3DE0C20776B269CF");

            entity.ToTable("Book");

            entity.Property(e => e.AddedAt).HasColumnType("datetime");
            entity.Property(e => e.BookDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BookName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BookPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ImageUrl1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImageURL1");
            entity.Property(e => e.ImageUrl2)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImageURL2");
            entity.Property(e => e.ImageUrl3)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImageURL3");
            entity.Property(e => e.ImageUrl4)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ImageURL4");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__Book__PublisherI__5FB337D6");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Books)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__Book__SupplierId__60A75C0F");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => e.BookAuthorId).HasName("PK__BookAuth__21B24F596F52E52A");

            entity.ToTable("BookAuthor");

            entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__BookAutho__Autho__68487DD7");

            entity.HasOne(d => d.Book).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__BookAutho__BookI__6754599E");
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(e => e.BookCategoryId).HasName("PK__BookCate__6347EC04460445DC");

            entity.ToTable("BookCategory");

            entity.HasOne(d => d.Book).WithMany(p => p.BookCategories)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__BookCateg__BookI__6383C8BA");

            entity.HasOne(d => d.Category).WithMany(p => p.BookCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__BookCateg__Categ__6477ECF3");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B7CC8ECED3");

            entity.ToTable("Cart");

            entity.HasOne(d => d.Book).WithMany(p => p.Carts)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__Cart__BookId__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Cart__UserId__6FE99F9F");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B653C7BA6");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD6CBCC90A3");

            entity.ToTable("Feedback");

            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Reply)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ReplyDate).HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__Feedback__BookId__7B5B524B");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Feedback__OrderI__7D439ABD");

            entity.HasOne(d => d.ReplyAccount).WithMany(p => p.FeedbackReplyAccounts)
                .HasForeignKey(d => d.ReplyAccountId)
                .HasConstraintName("FK__Feedback__ReplyA__7E37BEF6");

            entity.HasOne(d => d.User).WithMany(p => p.FeedbackUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Feedback__UserId__7C4F7684");
        });

        modelBuilder.Entity<ImportStock>(entity =>
        {
            entity.HasKey(e => e.ImportId).HasName("PK__ImportSt__869767EA96A5230D");

            entity.ToTable("ImportStock");

            entity.Property(e => e.ImportDate).HasColumnType("datetime");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ImportStocks)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__ImportSto__Suppl__01142BA1");

            entity.HasOne(d => d.User).WithMany(p => p.ImportStocks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ImportSto__UserI__02084FDA");
        });

        modelBuilder.Entity<ImportStockDetail>(entity =>
        {
            entity.HasKey(e => e.ImportStockDetailId).HasName("PK__ImportSt__CDCE9B8944C341D0");

            entity.ToTable("ImportStockDetail");

            entity.HasOne(d => d.Book).WithMany(p => p.ImportStockDetails)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__ImportSto__BookI__05D8E0BE");

            entity.HasOne(d => d.Import).WithMany(p => p.ImportStockDetails)
                .HasForeignKey(d => d.ImportId)
                .HasConstraintName("FK__ImportSto__Impor__04E4BC85");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCFA1CA5E40");

            entity.ToTable("Order");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DeliveredDate)
                .HasColumnType("datetime")
                .HasColumnName("deliveredDate");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order__UserId__74AE54BC");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36C49A5CD5D");

            entity.ToTable("OrderDetail");

            entity.HasOne(d => d.Book).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__OrderDeta__BookI__787EE5A0");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__778AC167");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657FABF65A5FFF");

            entity.ToTable("Publisher");

            entity.Property(e => e.PublisherName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A04A69FE7");

            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ__Role__737584F69AFC2421").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B4036063F0");

            entity.ToTable("Supplier");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SupplierEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SupplierName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C37809185");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E4F2C7A4D5").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534F3459E7E").IsUnique();

            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__AF2760AD95205167");

            entity.ToTable("UserRole");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__UserRole__RoleId__5535A963");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserRole__UserId__5441852A");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Voucher__3AEE79211D48A626");

            entity.ToTable("Voucher");

            entity.Property(e => e.ExpriryDate).HasColumnType("datetime");
            entity.Property(e => e.MaxOrderAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MinOrderAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.VoucherCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VoucherDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
