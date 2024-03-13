namespace ArchCore.Analytics
{
    public interface IAnalyticsService
    {
        void Fire(AnalyticsEvent analyticsEvent);
    }
}