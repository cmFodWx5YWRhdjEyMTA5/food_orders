using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HWOrderFood.Services.Analitics;

namespace HWOrderFood.Helpers
{
    public class ErrorsTracker
    {
        public static void TrackError(Exception Ex = null, string Message = null, Dictionary<string, string> dict = null, [CallerMemberNameAttribute]string CallerName = null, [CallerFilePath]string CallerFile = null, [CallerLineNumber]int CallerLineNumber = 0)
        {
#if Debug
            return;
#else
            return;
#endif

           // var analyticsService = App.Resolve<IAnalyticsService>();
            var param = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(CallerName))
                param[nameof(CallerName)] = CallerName;
            if (!string.IsNullOrEmpty(CallerFile))
                param[nameof(CallerFile)] = CallerFile;
            param[nameof(CallerLineNumber)] = CallerLineNumber.ToString();
            if (!string.IsNullOrEmpty((Message)))
                param[nameof(Message)] = Message;

            if (dict != null)
                foreach (var kv in dict)
                    param[kv.Key] = kv.Value;

            if (Ex != null)
                param["ExceptionType"] = Ex.GetType().Name;

            //analyticsService.Track("ErrorMessage", param);
        }
    }
}
