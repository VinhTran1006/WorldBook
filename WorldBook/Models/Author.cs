using System;
using System.Collections.Generic;

namespace WorldBook.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorDescription { get; set; }

    public bool? IsActive { get; set; } = true;

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
