using WorldBook.Models;

namespace WorldBook.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByOrderIdAsync(int orderId);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task UpdateAsync(Payment payment);
    }

}
