using System.Threading.Tasks;
using SaluteOnline.Shared.Events;

namespace SaluteOnline.MailService.Services.Declaration
{
    public interface ISesHandler
    {
        Task<bool> HandleEmail(SendEmailEvent email);
        Task<bool> HandleSupportEvent(ContactSupportEvent supportEvent);
    }
}