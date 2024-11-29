﻿using System.Threading.Tasks;
using OnlineContestManagement.Data.Models;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Infrastructure.Services
{
    public interface IDashboardService
    {
        Task<ContestStatisticsModel> GetContestStatisticsAsync();
        Task<RegistrationStatisticsModel> GetRegistrationStatisticsAsync();
        Task<int> GetTotalContestsAsync();
        Task<Dictionary<string, List<User>>> GetContestParticipantsAsync();
        Task<decimal> GetContestRevenueAsync();
        Task<decimal> GetWebsiteRevenueAsync();
        Task<int> GetTotalParticipantsAsync();

    }
}
