namespace WorldBook.ViewModel
{
    public class SearchResultViewModel
    {
        public string Query { get; set; } = string.Empty;
        public string Filter { get; set; } = "all";
        public List<BookDetailViewModel> Books { get; set; } = new();
        public int TotalResults => Books?.Count ?? 0;

        public bool HasResults => Books != null && Books.Any();
    }
}