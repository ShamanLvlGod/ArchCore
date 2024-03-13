using System;
using System.Collections.Generic;
using ArchCore.LocalStorage;

namespace ArchCore.Analytics
{
    public class BasicAnalyticsManager : IAnalyticsManager
    {

        private readonly List<IAnalyticsService> analyticsServices;

        public BasicAnalyticsManager(List<IAnalyticsService> analyticsServices)
        {
            this.analyticsServices = analyticsServices;
        }
        
        public void FireEvent(string key, Dictionary<string, object> data)
        {
            var analyticsEvent = new AnalyticsEvent()
            {
                registrationTime = DateTime.UtcNow.Second,
                key = key,
                data = data
            };
            
            foreach (var analyticsService in analyticsServices)
            {
                analyticsService.Fire(analyticsEvent);
            }
        }
    }
}