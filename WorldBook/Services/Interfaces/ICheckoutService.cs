// WorldBook/Services/Interfaces/ICheckoutService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutViewModel> GetCheckoutDataAsync(int userId, List<int> selectedBookIds);
        Task<List<AvailableVoucherViewModel>> GetAvailableVouchersAsync(int userId, decimal totalAmount);
        Task<CheckoutViewModel> ApplyVoucherAsync(CheckoutViewModel model, int voucherId);
        Task<int> CreateOrderAsync(CheckoutViewModel model);
    }
}