using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineContestManagement.Infrastructure.Services
{
    public class ContestRegistrationService : IContestRegistrationService
    {
        private readonly IContestRegistrationRepository _registrationRepository;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly IContestService _contestService;

        public ContestRegistrationService(IContestRegistrationRepository registrationRepository, IEmailService emailService, IPaymentService paymentService, IContestService contestService)
        {
            _registrationRepository = registrationRepository;
            _emailService = emailService;
            _paymentService = paymentService;
            _contestService = contestService;
        }



        public async Task<RegistrationResult> RegisterUserForContestAsync(string contestId, RegisterForContestModel registrationModel)
        {
            try
            {
                var existingRegistration = await _registrationRepository.GetRegistrationByUserIdAndContestIdAsync(
                    contestId,
                    registrationModel.UserId
                );

                if (existingRegistration != null)
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "User is already registered for this contest"
                    };
                }

                var contest = await _contestService.GetContestDetailsAsync(contestId);
                if (contest == null)
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "Contest not found"
                    };
                }

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
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "Payment creation failed"
                    };
                }

                var registration = new ContestRegistration
                {
                    ContestId = contest.Id,
                    UserId = registrationModel.UserId,
                    Name = registrationModel.Name,
                    DateOfBirth = registrationModel.DateOfBirth,
                    Email = registrationModel.Email,
                    AdditionalInfo = registrationModel.AdditionalInfo,
                    RegistrationDate = DateTime.UtcNow,
                    Status = "Pending",
                };

                var result = await _registrationRepository.RegisterUserAsync(registration);
                if (!result)
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        Error = "Failed to save registration"
                    };
                }

                await _emailService.SendRegistrationConfirmation(registration.Email, registration.ContestId);

                return new RegistrationResult
                {
                    Success = true,
                    PaymentLink = paymentResult.PaymentLink,
                    Message = "Registration pending payment confirmation"
                };
            }
            catch (Exception)
            {
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
    }
}
