using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Domain.Mongo;

namespace SaluteOnline.API.Services.Implementation
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(IUnitOfWork unitOfWork, ILogger<ActivityService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
