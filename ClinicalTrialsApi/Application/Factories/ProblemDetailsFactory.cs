using Json.Schema;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ClinicalTrialsApi.Application.Factories
{
    public static class ProblemDetailsFactory
    {
        public static ProblemDetails CreateInternalServerError(string message)
        {
            return new ProblemDetails
            {
                Detail = message,
                Instance = "api",
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Internal Server Error",
            };
        }
        public static ProblemDetails CreateNotFound(string message)
        {
            return new ProblemDetails
            {
                Detail = message,
                Instance = "api",
                Status = (int)HttpStatusCode.NotFound,
                Title = "Not Found",
            };   
        }

        public static ProblemDetails CreateBadRequest(string message)
        {
            return new ProblemDetails
            {
                Detail = message,
                Instance = "api",
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad Request",
            };
        }

        public static ProblemDetails CreateValidationErrors(EvaluationResults evaluationResults)
        {
            return new ProblemDetails
            {
                Detail = "Validation Error",
                Instance = "api",
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Extensions = CreateValidationErrorDetails(evaluationResults)
            };
        }

        private static Dictionary<string, object?> CreateValidationErrorDetails(EvaluationResults results)
        {
            var errorsDictionary = results
                .Details
                .Where(x => x.Errors != null)
                .ToDictionary(
                    x => x.EvaluationPath.ToString().Split('/').Last(), 
                    x => x.Errors?.Values);

            return new Dictionary<string, object?>
            {
                { "errors", errorsDictionary }
            };
        }
    }
}
