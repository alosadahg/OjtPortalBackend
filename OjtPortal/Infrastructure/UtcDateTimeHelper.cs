namespace OjtPortal.Infrastructure
{
    public static class UtcDateTimeHelper
    {
        public static DateTime FromUtcToLocal(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local);
        }
    }
}
