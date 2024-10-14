namespace OjtPortal.Dtos
{
    public class SentimentAnalysisDto
    {
        public string Label { get; set; } = string.Empty;
        public double Score { get; set; } = 0;
    }

    public class LogbookRemarksRequest
    {
        public string Remarks { get; set; } = string.Empty;
    }
}
