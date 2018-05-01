using SaluteOnline.Shared.Common;

namespace SaluteOnline.ConfigurationService.Domain
{
    public class DefaultDashboardConfiguration : DashboardConfiguration
    {
        public Roles ForRole { get; set; }
    }
}
