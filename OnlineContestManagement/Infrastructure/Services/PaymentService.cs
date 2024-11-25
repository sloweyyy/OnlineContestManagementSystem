using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;


namespace OnlineContestManagement.Infrastructure.Services
{
  public class PaymentService : IPaymentService
  {
    private readonly PayOSSettings _payOSSettings;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(PayOSSettings payOSSettings, IPaymentRepository paymentRepository)
    {
      _payOSSettings = payOSSettings;
      _paymentRepository = paymentRepository;
    }


    private PayOS _payOS => new PayOS(_payOSSettings.ClientId, _payOSSettings.ApiKey, _payOSSettings.ChecksumKey);

    public async Task<Payment> CreatePaymentAsync(Payment paymentModel)
    {
      int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
      ItemData item = new ItemData(paymentModel.ProductName, 1, (int)paymentModel.Price);
      List<ItemData> items = [item];
      PaymentData paymentData = new PaymentData(orderCode, (int)paymentModel.Price, paymentModel.Description, items, paymentModel.CancelUrl, paymentModel.ReturnUrl);

      CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

      paymentModel.OrderId = orderCode;
      paymentModel.PaymentLink = createPayment.checkoutUrl;
      paymentModel.Status = "pending";
      await _paymentRepository.CreatePaymentAsync(paymentModel);


      return paymentModel;
    }

    public async Task<PaymentLinkInformation> GetPaymentInformationAsync(int orderId)
    {
      return await _payOS.getPaymentLinkInformation(orderId);
    }

    public async Task<PaymentLinkInformation> CancelPaymentAsync(int orderId)
    {
      return await _payOS.cancelPaymentLink(orderId);
    }
    public async Task<Payment> GetPaymentByContestIdAndUserIdAsync(string contestId, string userId)
    {
      return await _paymentRepository.GetPaymentByContestIdAndUserIdAsync(contestId, userId);
    }
  }
}