namespace WorldBook.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendWelcomeEmailAsync(string email, string username, string password, string fullName);
        Task<bool> SendProfileCompletionReminderAsync(string email, string username, string fullName);
    }
}
    
