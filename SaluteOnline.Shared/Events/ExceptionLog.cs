using System;
using Newtonsoft.Json;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.Shared.Events
{
    public class ExceptionLog
    {
        public ExceptionLog(Exception exception, object data, string userId, ActivityImportance importance, string controller, string method)
        {
            Exception = JsonConvert.SerializeObject(exception);
            Data = JsonConvert.SerializeObject(data);
            UserId = userId;
            Importance = importance;
            Controller = controller;
            Method = method;
        }

        public string Exception { get; set; }
        public string Data { get; set; }
        public string UserId { get; set; }
        public ActivityImportance Importance { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
    }
}
