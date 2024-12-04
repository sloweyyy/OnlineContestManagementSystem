using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using OnlineContestManagement.Data.Models;

public interface IPaymentService
{
  Task<Payment> CreatePaymentAsync(Payment payment);
  Task<PaymentLinkInformation> GetPaymentInformationAsync(int orderId);
  Task<PaymentLinkInformation> CancelPaymentAsync(int orderId);
  Task<Payment> GetPaymentByContestIdAndUserIdAsync(string contestId, string userId);
  Task updatePaymentStatus(string contestId, string userId, string status);
  Task<IActionResult> UpdateAllPaymentStatusesAsync();


}
