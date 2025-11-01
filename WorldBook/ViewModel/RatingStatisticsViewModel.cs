namespace WorldBook.ViewModel
{
    public class RatingStatisticsViewModel
    {
        public int TotalReviews { get; set; }
        public decimal AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new()
    {
        { 5, 0 },
        { 4, 0 },
        { 3, 0 },
        { 2, 0 },
        { 1, 0 }
    };
    }
}
