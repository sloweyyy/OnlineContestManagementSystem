using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace OnlineContestManagement.Infrastructure.Services
{
  public class PaymentService : IPaymentService
  {
    private readonly PayOSSettings _payOSSettings;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(PayOSSettings payOSSettings, IPaymentRepository paymentRepository, ILogger<PaymentService> logger)
    {
      _payOSSettings = payOSSettings;
      _paymentRepository = paymentRepository;
      _logger = logger;
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

    public async Task updatePaymentStatus(string contestId, string userId, string status)
    {
      Payment payment = await _paymentRepository.GetPaymentByContestIdAndUserIdAsync(contestId, userId);
      payment.Status = status.ToLower();
      payment.UpdatedAt = DateTime.UtcNow;
      await _paymentRepository.UpdatePaymentAsync(payment);
    }

    public async Task<IActionResult> UpdateAllPaymentStatusesAsync()
    {
      try
      {
        var payments = await _paymentRepository.GetAllPaymentsAsync();
        foreach (var payment in payments)
        {
          string rawResponse = string.Empty;
          try
          {
            var paymentInfo = await _payOS.getPaymentLinkInformation(payment.OrderId);
            _logger.LogInformation("PaymentLinkInformation for OrderId {OrderId}: {@PaymentInfo}", payment.OrderId, paymentInfo);

            if (paymentInfo.status.ToLower() != payment.Status.ToLower())
            {
              payment.Status = paymentInfo.status.ToLower();
              payment.UpdatedAt = DateTime.UtcNow;
              await _paymentRepository.UpdatePaymentAsync(payment);
              _logger.LogInformation("Updated payment {PaymentId} status to {Status}", payment.Id, payment.Status);
            }
          }
          catch (JsonReaderException jsonEx)
          {
            _logger.LogError(jsonEx, "JSON Parsing Error for OrderId {OrderId}. Raw Response: {RawResponse}", payment.OrderId, rawResponse);
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, "Error retrieving payment information for OrderId {OrderId}", payment.OrderId);
          }
        }
        return new OkObjectResult("All payment statuses updated successfully.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error updating payment statuses.");
        return new BadRequestObjectResult(new { Message = "Error updating payment statuses", Error = ex.Message });
      }
    }
  }
}