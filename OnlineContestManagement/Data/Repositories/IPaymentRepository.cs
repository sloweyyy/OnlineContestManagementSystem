using OnlineContestManagement.Data.Models;

namespace OnlineContestManagement.Data.Repositories
{
  public interface IPaymentRepository
  {
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<Payment> GetPaymentByContestIdAndUserIdAsync(string contestId, string userId);
    Task<decimal> GetTotalRevenueAsync();
    Task UpdatePaymentAsync(Payment payment);
    Task<List<Payment>> GetAllPaymentsAsync();

    Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync();

  }
}