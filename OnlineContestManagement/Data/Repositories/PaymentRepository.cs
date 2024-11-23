using MongoDB.Driver;
using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Data.Repositories
{
  public class PaymentRepository : IPaymentRepository
  {
    private readonly IMongoCollection<Payment> _payments;

    public PaymentRepository(IMongoDatabase database)
    {
      _payments = database.GetCollection<Payment>("Payments");
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
      await _payments.InsertOneAsync(payment);
      return payment;
    }
  }
}