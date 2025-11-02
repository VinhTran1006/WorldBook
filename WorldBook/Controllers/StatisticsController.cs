using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldBook.Services.Interfaces;

namespace WorldBook.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var statistics = await _statisticsService.GetDashboardStatisticsAsync();
            return View("~/Views/AdminViews/Statistics/Dashboard.cshtml", statistics);
        }
    }
}