using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Models;
using System;
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
                await _emailService.SendRegistrationConfirmation(registrationModel.Email, registrationModel.ContestId);
            }
            return result;
        }

        public async Task<bool> WithdrawUserFromContestAsync(WithdrawFromContestModel withdrawModel)
        {
            return await _registrationRepository.WithdrawUserAsync(withdrawModel.ContestId, withdrawModel.UserId);
        }

        public async Task<List<ContestRegistration>> SearchRegistrationsAsync(ContestRegistrationSearchFilter filter)
        {
            return await _registrationRepository.SearchRegistrationsAsync(filter);
        }
    }
}
