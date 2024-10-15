using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Services;
using System.Security.Policy;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/sentiment/analysis")]
    public class SentimentAnalysisController : OjtPortalBaseController
    {
        private readonly ISentimentalAnalysisService _sentimentalAnalysisService;

        public SentimentAnalysisController(ISentimentalAnalysisService sentimentalAnalysisService)
        {
            this._sentimentalAnalysisService = sentimentalAnalysisService;
        }

        /// <summary>
        /// Get the sentiment analysis 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetSentimentAnalysis([FromBody] string input)
        {
            var (result, error) = await _sentimentalAnalysisService.AnalyzeSentimentAsync (input);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}
