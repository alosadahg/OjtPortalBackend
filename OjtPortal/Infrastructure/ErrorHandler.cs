using Microsoft.AspNetCore.Identity;
using System.Net;

namespace OjtPortal.Infrastructure
{

    public static class ErrorHandler
    {

        public static ErrorResponseModel getIdentityErrorResponse(IEnumerable<IdentityError> errors, string? email)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            foreach (var error in errors)
            {
                ErrorModel errorModel = error.Code.Equals("DuplicateUserName")
                    ? new ErrorModel("Invalid Email", "Email '" + email + "' is already taken.")
                    : new ErrorModel(error.Code, error.Description);
                errorResponseModel.Errors.Add(errorModel);
            }
            errorResponseModel.statusCode = HttpStatusCode.BadRequest;
            return errorResponseModel;
        }

        public static ErrorModel getActivationError(HttpStatusCode status, string email)
        {
            ErrorModel errorModel = new ErrorModel("Activation Email Not Delivered", "Cannot send activation email to '" + email + "'");
            if (status.Equals(HttpStatusCode.NotFound))
            {
                errorModel.message = "No record is associated for this email.";
            }
            return errorModel;
        }
    }

    public class ErrorResponseModel
    {
        public HttpStatusCode statusCode { get; set; }
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

        public ErrorResponseModel()
        {
        }

        public ErrorResponseModel(HttpStatusCode statusCode, ErrorModel error)
        {
            this.statusCode = statusCode;
            Errors.Add(error);
        }

        public ErrorResponseModel(HttpStatusCode statusCode, string code, string message)
        {
            this.statusCode = statusCode;
            Errors.Add(new ErrorModel(code, message));
        }

        public ErrorResponseModel(HttpStatusCode statusCode, List<ErrorModel> errors)
        {
            this.statusCode = statusCode;
            Errors = errors;
        }
    }

    public class ErrorModel
    {
        public string? code { get; set; }
        public string message { get; set; }

        public ErrorModel(string? code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }
}
