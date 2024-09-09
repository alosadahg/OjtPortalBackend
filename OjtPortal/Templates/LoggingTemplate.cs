namespace OjtPortal.EmailTemplates
{
    public static class LoggingTemplate
    {
        public static string CacheMissProceedToDatabase(string source)
        {
            return $"No existing {source} in cache. Attempting to find in database...";
        }
    }
}
