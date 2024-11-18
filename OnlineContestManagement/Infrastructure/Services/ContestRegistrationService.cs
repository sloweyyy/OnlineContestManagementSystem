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

        public ContestRegistrationService(IContestRegistrationRepository registrationRepository, IEmailService emailService)
        {
            _registrationRepository = registrationRepository;
            _emailService = emailService;
        }

        public async Task<bool> RegisterUserForContestAsync(RegisterForContestModel registrationModel)
        {
            var registration = new ContestRegistration
            {
                ContestId = registrationModel.ContestId,
                UserId = registrationModel.UserId,
                Name = registrationModel.Name,
                DateOfBirth = registrationModel.DateOfBirth,
                Email = registrationModel.Email,
                AdditionalInfo = registrationModel.AdditionalInfo,
                RegistrationDate = DateTime.UtcNow,
                Status = "Registered"
            };

            var result = await _registrationRepository.RegisterUserAsync(registration);
            if (result)
            {
                Console.WriteLine("Sending registration confirmation email to " + registrationModel.Email);
                await _emailService.SendRegistrationConfirmation(registrationModel.Email, registrationModel.ContestId);
            }
            return result;
        }

        public async Task<bool> WithdrawUserFromContestAsync(WithdrawFromContestModel withdrawModel)
        {
            await _registrationRepository.WithdrawUserAsync(withdrawModel.ContestId, withdrawModel.UserId);

            var registration = await _registrationRepository.GetRegistrationByUserIdAndContestIdAsync(withdrawModel.ContestId, withdrawModel.UserId);
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
