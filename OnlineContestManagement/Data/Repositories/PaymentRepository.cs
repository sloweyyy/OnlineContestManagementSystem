using MongoDB.Bson;
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
                var payments = await _payments.Find(p => p.ContestId != null && p.Status.ToLower() != "cancelled").ToListAsync();
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
        public async Task UpdatePaymentAsync(Payment payment)
        {
            await _payments.ReplaceOneAsync(p => p.Id == payment.Id, payment);
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _payments.Find(FilterDefinition<Payment>.Empty).ToListAsync();
        }
        public async Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync()
        {
            var currentYear = DateTime.UtcNow.Year;
            var lastYear = currentYear - 1;

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match", new BsonDocument
            {
                { "CreatedAt", new BsonDocument("$gte", new DateTime(lastYear, 1, 1)) }
            }),
        new BsonDocument("$project", new BsonDocument
            {
                { "Year", new BsonDocument("$year", "$CreatedAt") },
                { "Month", new BsonDocument("$month", "$CreatedAt") },
                { "Price", 1 }
            }),
        new BsonDocument("$group", new BsonDocument
            {
                { "_id", new BsonDocument
                    {
                        { "Year", "$Year" },
                        { "Month", "$Month" }
                    }
                },
                { "TotalRevenue", new BsonDocument("$sum", "$Price") }
            }),
        new BsonDocument("$sort", new BsonDocument
            {
                { "_id.Year", 1 },
                { "_id.Month", 1 }
            })
            };

            var result = await _payments.Aggregate<MonthlyRevenue>(pipeline).ToListAsync();
            return result;
        }
    }
}