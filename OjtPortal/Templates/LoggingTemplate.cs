namespace OjtPortal.EmailTemplates
{
    public static class LoggingTemplate
    {
        public static string CacheMissProceedToDatabase(string type)
        {
            return $"No existing {type} in cache. Attempting to find in database...";
        }

        public static string DuplicateRecordTitle(string type)
        {
            return $"Duplicate {type} exists.";
        }

        public static string DuplicateRecordDescription(string type, string source)
        {
            return $"Existing {type} record found for: {source}";
        }

        public static string MissingRecordTitle(string type)
        {
            return $"Missing {type} record.";
        }

        public static string MissingRecordDescription(string type, string source)
        {
            return $"No {type} record found for: {source}";
        }

    }
}
