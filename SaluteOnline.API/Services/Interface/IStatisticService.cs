using System.Collections.Generic;
using SaluteOnline.API.DTO.Statistic;

namespace SaluteOnline.API.Services.Interface
{
    public interface IStatisticService
    {
        IEnumerable<ClubDashboardStatisticDto> GetMyClubsStatistics(string subjectId);
    }
}