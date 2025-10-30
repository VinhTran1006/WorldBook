using Microsoft.EntityFrameworkCore;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;

namespace WorldBook.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly WorldBookDbContext _db;
        public PaymentRepository(WorldBookDbContext db) { _db = db; }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByOrderIdAsync(int orderId)
            => await _db.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
            => await _db.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);

        public async Task UpdateAsync(Payment payment)
        {
            _db.Payments.Update(payment);
            await _db.SaveChangesAsync();
        }
    }
}
