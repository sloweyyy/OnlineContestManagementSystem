using MongoDB.Driver;
using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Data.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Payment> _payments;

        public PaymentRepository(IMongoDatabase database)
        {
            _payments = database?.GetCollection<Payment>("Payments")
                        ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _payments.InsertOneAsync(payment);
            return payment;
        }

        public async Task<Payment> GetPaymentByContestIdAndUserIdAsync(string contestId, string userId)
        {
            return await _payments.Find(p => p.ContestId == contestId && p.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            try
            {
                var payments = await _payments.Find(p => p.ContestId != null && p.Status == "Completed").ToListAsync();
                if (payments == null || !payments.Any())
                {
                    Console.WriteLine("No payments found with ContestId and Status = Completed.");
                    return 0;
                }

                Console.WriteLine($"Found {payments.Count} payments. Calculating total revenue.");
                return payments.Sum(p => p.Price);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalRevenueAsync: {ex.Message}");
                throw;
            }
        }



    }
}