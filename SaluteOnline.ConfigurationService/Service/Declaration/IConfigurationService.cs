using System.Collections.Generic;
using SaluteOnline.ConfigurationService.Domain;

namespace SaluteOnline.ConfigurationService.Service.Declaration
{
    public interface IConfigurationService
    {
        void SaveDashboardConfiguration(List<DashboardConfigurationItem> panels, string subjectId);
        DashboardConfiguration GetDashboardConfiguration(string subjectId, string role);

        void SaveClubDashboardConfiguration(ClubDashboardConfiguration configuration, string subjectId);
        ClubDashboardConfiguration GetClubDashboardConfiguration(string subjectId, string role, int clubId);
    }
}