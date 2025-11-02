using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<StatisticsViewModel> GetDashboardStatisticsAsync();
    }
}