using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IContestRegistrationService
    {
        Task<bool> RegisterUserForContestAsync(RegisterForContestModel registrationModel);
        Task<bool> WithdrawUserFromContestAsync(WithdrawFromContestModel withdrawModel);
        Task<List<ContestRegistration>> SearchRegistrationsAsync(ContestRegistrationSearchFilter filter);
        
    }

}
