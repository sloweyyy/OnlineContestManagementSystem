using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace OnlineContestManagement.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PaymentController : ControllerBase
  {
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
      _paymentService = paymentService;
    }

    [HttpGet("get-payment-status/{contestId}/{userId}")]
    public async Task<ActionResult<PaymentLinkInformation>> GetPaymentLinkInformation(string contestId, string userId)
    {
      try
      {
        var payment = await _paymentService.GetPaymentByContestIdAndUserIdAsync(contestId, userId);
        if (payment == null)
        {
          return NotFound(new { Message = "Payment not found" });
        }
        var paymentLinkInformation = await _paymentService.GetPaymentInformationAsync(payment.OrderId);
        if (paymentLinkInformation.status != "pending")
        {
          await _paymentService.updatePaymentStatus(contestId, userId, paymentLinkInformation.status);
        }
        return Ok(paymentLinkInformation);
      }
      catch (Exception ex)
      {
        return BadRequest(new { Message = "Error retrieving payment information", Error = ex.Message });
      }
    }

    [HttpPut("update-all-payment-statuses")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAllPaymentStatuses()
    {
      return await _paymentService.UpdateAllPaymentStatusesAsync();
    }

  }
}