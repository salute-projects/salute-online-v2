using System.Threading.Tasks;
using SaluteOnline.Domain.Domain.Mongo;

namespace SaluteOnline.API.Services.Interface
{
    public interface IActivityService
    {
        void LogActivity(Activity activity);
    }
}