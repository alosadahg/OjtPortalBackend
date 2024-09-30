using Microsoft.AspNetCore.Identity;
using System.Net;

namespace OjtPortal.Infrastructure
{

    public static class ErrorHandler
    {

        public static ErrorResponseModel GetIdentityErrorResponse(IEnumerable<IdentityError> errors, string? email)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            foreach (var error in errors)
            {
                ErrorModel errorModel = error.Code.Equals("DuplicateUserName")
                    ? new ErrorModel("Invalid Email", "Email '" + email + "' is already taken.")
                    : new ErrorModel(error.Code, error.Description);
                errorResponseModel.Errors.Add(errorModel);
            }
            errorResponseModel.StatusCode = HttpStatusCode.BadRequest;
            return errorResponseModel;
        }

        public static ErrorModel GetActivationError(HttpStatusCode status, string email)
        {
            ErrorModel errorModel = new ErrorModel("Activation Email Not Delivered", "Cannot send activation email to '" + email + "'");
            if (status.Equals(HttpStatusCode.NotFound))
            {
                errorModel.Message = "No record is associated for this email.";
            }
            return errorModel;
        }
    }

    public class ErrorResponseModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

        public ErrorResponseModel()
        {
        }

        public ErrorResponseModel(HttpStatusCode statusCode, ErrorModel error)
        {
            this.StatusCode = statusCode;
            Errors.Add(error);
        }

        public ErrorResponseModel(HttpStatusCode statusCode, string code, string message)
        {
            this.StatusCode = statusCode;
            Errors.Add(new ErrorModel(code, message));
        }

        public ErrorResponseModel(HttpStatusCode statusCode, List<ErrorModel> errors)
        {
            this.StatusCode = statusCode;
            Errors = errors;
        }
    }

    public class ErrorModel
    {
        public string? Code { get; set; }
        public string Message { get; set; }

        public ErrorModel(string? code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}
