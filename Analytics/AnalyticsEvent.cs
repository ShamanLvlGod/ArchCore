using System.Collections.Generic;

namespace ArchCore.Analytics
{
    public class AnalyticsEvent
    {
        public int registrationTime;
        public string key;
        public Dictionary<string, object> data;
    }
}