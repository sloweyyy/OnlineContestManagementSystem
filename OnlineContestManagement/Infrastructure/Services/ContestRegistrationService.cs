using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;
using System.Globalization;
using ClosedXML.Excel;



namespace OnlineContestManagement.Infrastructure.Services
{
    public class ContestRegistrationService : IContestRegistrationService
    {
        private readonly IContestRegistrationRepository _registrationRepository;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly IContestService _contestService;
        private readonly ILogger<ContestRegistrationService> _logger;

        public ContestRegistrationService(IContestRegistrationRepository registrationRepository, IEmailService emailService, IPaymentService paymentService, IContestService contestService, ILogger<ContestRegistrationService> logger)
        {
            _registrationRepository = registrationRepository;
            _emailService = emailService;
            _paymentService = paymentService;
            _contestService = contestService;
            _logger = logger;

        }



        public async Task<RegistrationResult> RegisterUserForContestAsync(string contestId, RegisterForContestModel registrationModel)
        {
            try
            {
                _logger.LogInformation(
                    "Starting registration process for User {UserId} in Contest {ContestId}",
                    registrationModel.UserId,
                    contestId
                );

                var existingRegistration = await _registrationRepository.GetRegistrationByUserIdAndContestIdAsync(
                    contestId,
                    registrationModel.UserId
                );

                if (existingRegistration != null)
                {
                    _logger.LogWarning(
                        "Duplicate registration attempt - User {UserId} is already registered for Contest {ContestId}",
                        registrationModel.UserId,
                        contestId
                    );
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "User is already registered for this contest"
                    };
                }

                var contest = await _contestService.GetContestDetailsAsync(contestId);
                if (contest == null)
                {
                    _logger.LogError(
                        "Contest {ContestId} not found during registration attempt for User {UserId}",
                        contestId,
                        registrationModel.UserId
                    );
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "Contest not found"
                    };
                }

                _logger.LogInformation(
                    "Creating payment for Contest {ContestId}, User {UserId}, Amount {Amount}",
                    contestId,
                    registrationModel.UserId,
                    contest.EntryFee
                );

                var payment = new Payment
                {
                    ProductName = contest.Name,
                    Price = contest.EntryFee,
                    Description = contest.Id,
                    CancelUrl = "your_cancel_url",
                    ReturnUrl = "your_return_url",
                    ContestId = contest.Id,
                    UserId = registrationModel.UserId
                };

                var paymentResult = await _paymentService.CreatePaymentAsync(payment);
                if (paymentResult == null)
                {
                    _logger.LogError(
                        "Payment creation failed for User {UserId}, Contest {ContestId}",
                        registrationModel.UserId,
                        contestId
                    );
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "Payment creation failed"
                    };
                }

                var registration = new ContestRegistration
                {
                    ContestId = contestId,
                    UserId = registrationModel.UserId,
                    Name = registrationModel.Name,
                    DateOfBirth = registrationModel.DateOfBirth,
                    Email = registrationModel.Email,
                    AdditionalInfo = registrationModel.AdditionalInfo,
                    RegistrationDate = DateTime.UtcNow,
                    Status = "Pending",
                };

                _logger.LogInformation(
                    "Saving registration for User {UserId} in Contest {ContestId}",
                    registration.UserId,
                    registration.ContestId
                );

                var result = await _registrationRepository.RegisterUserAsync(registration);
                if (!result)
                {
                    _logger.LogError(
                        "Failed to save registration for User {UserId} in Contest {ContestId}",
                        registration.UserId,
                        registration.ContestId
                    );
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "Failed to save registration"
                    };
                }

                try
                {
                    await _emailService.SendRegistrationConfirmation(registration.Email, registration.ContestId);
                    _logger.LogInformation(
                        "Registration confirmation email sent to {Email} for Contest {ContestId}",
                        registration.Email,
                        registration.ContestId
                    );
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(
                        emailEx,
                        "Failed to send registration confirmation email to {Email} for Contest {ContestId}",
                        registration.Email,
                        registration.ContestId
                    );
                    // Continue execution as email sending failure shouldn't block registration
                }

                _logger.LogInformation(
                    "Registration completed successfully for User {UserId} in Contest {ContestId}",
                    registration.UserId,
                    registration.ContestId
                );

                return new RegistrationResult
                {
                    Success = true,
                    PaymentLink = paymentResult.PaymentLink,
                    Message = "Registration pending payment confirmation"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error during registration process for User {UserId} in Contest {ContestId}",
                    registrationModel.UserId,
                    contestId
                );
                return new RegistrationResult
                {
                    Success = false,
                    Error = "An unexpected error occurred during registration"
                };
            }
        }
        public async Task<bool> WithdrawUserFromContestAsync(WithdrawFromContestModel withdrawModel)
        {
            var registration = await _registrationRepository.GetRegistrationByUserIdAndContestIdAsync(withdrawModel.ContestId, withdrawModel.UserId);

            if (registration == null || registration.Status == "Withdrawn")
            {
                return false;
            }

            await _registrationRepository.WithdrawUserAsync(withdrawModel.ContestId, withdrawModel.UserId);

            if (registration != null)
            {
                await _emailService.SendWithdrawalConfirmation(registration.Email, withdrawModel.ContestId);
            }

            return true;
        }


        public async Task<List<ContestRegistration>> SearchRegistrationsAsync(ContestRegistrationSearchFilter filter)
        {
            return await _registrationRepository.SearchRegistrationsAsync(filter);
        }


        public async Task<List<ContestRegistration>> GetContestsByUserIdAsync(string userId)
        {
            return await _registrationRepository.GetRegistrationsByUserIdAsync(userId);
        }

        public async Task<List<ContestRegistration>> GetRegistrationsByContestIdAsync(string contestId)
        {
            return await _registrationRepository.GetRegistrationsByContestIdAsync(contestId);
        }

        public async Task UpdateRegistrationStatusAsync(string contestId, string userId, string status)
        {
            var registration = await _registrationRepository.GetRegistrationByUserIdAndContestIdAsync(contestId, userId);
            if (registration != null)
            {
                if (status.Equals("withdrawn", StringComparison.OrdinalIgnoreCase))
                    return;

                registration.Status = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status.ToLowerInvariant());
                await _registrationRepository.UpdateRegistrationAsync(registration);
            }
        }

        public async Task<byte[]> GenerateContestRegistrationsExcelAsync(string contestId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(contestId))
            {
                throw new ArgumentException("Contest ID cannot be null or empty.", nameof(contestId));
            }

            try
            {
                // Fetch contest details
                var contest = await _contestService.GetContestDetailsAsync(contestId);
                if (contest == null)
                {
                    throw new ArgumentException($"Contest with ID {contestId} not found.");
                }

                // Fetch registrations
                var registrations = await _registrationRepository.GetRegistrationsByContestIdAsync(contestId);

                // If no registrations, return a blank workbook
                if (registrations == null || !registrations.Any())
                {
                    using var emptyWorkbook = new XLWorkbook();
                    var emptyWorksheet = emptyWorkbook.Worksheets.Add("Registrations");

                    // Add a message about no registrations
                    emptyWorksheet.Cell("A1").Value = "No registrations found for this contest.";

                    using var emptyStream = new MemoryStream();
                    emptyWorkbook.SaveAs(emptyStream);
                    return emptyStream.ToArray();
                }

                // Create workbook
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Registrations");

                // Add contest information at the top
                worksheet.Cell("A1").Value = "Contest Name:";
                worksheet.Cell("B1").Value = contest.Name;
                worksheet.Cell("A2").Value = "Start Date:";
                worksheet.Cell("B2").Value = contest.StartDate.ToString("yyyy-MM-dd");
                worksheet.Cell("A3").Value = "End Date:";
                worksheet.Cell("B3").Value = contest.EndDate.ToString("yyyy-MM-dd");
                worksheet.Cell("A4").Value = "Total Registrations:";
                worksheet.Cell("B4").Value = registrations.Count();

                // Add headers for registrations
                string[] headers = new[] {
            "User ID", "Name", "Date of Birth",
            "Email", "Registration Date", "Status"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(5, i + 1).Value = headers[i];
                }

                // Style headers
                var headerRange = worksheet.Range(5, 1, 5, headers.Length);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // Populate registration data
                int row = 6;
                foreach (var reg in registrations)
                {
                    worksheet.Cell(row, 1).Value = reg.UserId;
                    worksheet.Cell(row, 2).Value = reg.Name;
                    worksheet.Cell(row, 3).Value = reg.DateOfBirth.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 4).Value = reg.Email;
                    worksheet.Cell(row, 5).Value = reg.RegistrationDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 6).Value = reg.Status;
                    row++;
                }

                // Auto-fit columns and add some styling
                worksheet.Columns().AdjustToContents();
                worksheet.Columns().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                // Add filter to headers
                worksheet.Range(5, 1, 5, headers.Length).SetAutoFilter();

                // Save to memory stream
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                // Log the full exception details
                _logger.LogError(ex, $"Error generating Excel for contest {contestId}");

                // Rethrow to allow caller to handle
                throw new ApplicationException("Failed to generate contest registrations Excel.", ex);
            }
        }
    }
}
