using Microsoft.AspNetCore.Mvc;
using OjtPortal.Infrastructure;
using System.Net;
using System.Text.Json;

namespace OjtPortal.Controllers.BaseController.cs
{
    public class OjtPortalBaseController : ControllerBase
    {
        protected IActionResult MakeErrorResponse(ErrorResponseModel? errorResponseModel)
        {
            HttpStatusCode code = errorResponseModel!.statusCode;
            return code switch
            {
                HttpStatusCode.BadRequest => BadRequest(errorResponseModel),
                HttpStatusCode.NotFound => NotFound(errorResponseModel),
                HttpStatusCode.Conflict => Conflict(errorResponseModel),
                HttpStatusCode.Unauthorized => Unauthorized(errorResponseModel),
                HttpStatusCode.UnprocessableEntity => UnprocessableEntity(errorResponseModel),
                HttpStatusCode.InternalServerError => Problem(JsonSerializer.Serialize(errorResponseModel), statusCode: 500),
                _ => StatusCode(((int) errorResponseModel.statusCode), errorResponseModel)
            };
        }
    }
}
