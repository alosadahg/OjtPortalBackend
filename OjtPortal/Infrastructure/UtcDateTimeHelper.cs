namespace OjtPortal.Infrastructure
{
    public static class UtcDateTimeHelper
    {
        public static DateTime FromUtcToLocal(DateTime dateTime)
        {
            var manilaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var currentDateTimeInManila = TimeZoneInfo.ConvertTime(dateTime, manilaTimeZone);
            return currentDateTimeInManila;
        }
    }
}
