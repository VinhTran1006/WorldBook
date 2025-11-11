using WorldBook.ViewModel;

namespace WorldBook.Services.Interfaces
{
    public interface IRegisterService
    {
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phone);
        Task RegisterUserAsync(RegisterViewModel model);
    }
}
