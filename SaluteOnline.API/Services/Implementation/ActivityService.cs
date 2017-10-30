using System;
using System.Threading.Tasks;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Domain.Mongo;

namespace SaluteOnline.API.Services.Implementation
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void LogActivity(Activity activity)
        {
            if (activity == null)
                return;
            Task.Run(() =>
            {
                activity.Guid = Guid.NewGuid();
                activity.Created = DateTimeOffset.UtcNow;
                _unitOfWork.Activities.Insert(activity);
            });
        }
    }
}
