using Net.payOS.Types;
using OnlineContestManagement.Data.Models;

public interface IPaymentService
{
  Task<Payment> CreatePaymentAsync(Payment payment);
  Task<PaymentLinkInformation> GetPaymentInformationAsync(int orderId);
  Task<PaymentLinkInformation> CancelPaymentAsync(int orderId);

}
