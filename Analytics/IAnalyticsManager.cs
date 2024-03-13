using System.Collections.Generic;

namespace ArchCore.Analytics
{
    public interface IAnalyticsManager
    {
        void FireEvent(string key, Dictionary<string, object> data);
    }
}