using Json.Schema;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ClinicalTrialsApi.Application.Factories
{
    public static class ServiceResultFactory
    {
        public static ServiceResult<T> CreateInternalServerError<T>(string message)
        {
            return ServiceResult<T>.FromError(new ProblemDetails
            {
                Detail = message,
                Instance = "api",
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Internal Server Error",
            });
        }
        public static ServiceResult<T> CreateNotFound<T>(string message)
        {
            return ServiceResult<T>.FromError(new ProblemDetails
            {
                Detail = message,
                Instance = "api",
                Status = (int)HttpStatusCode.NotFound,
                Title = "Not Found",
            });   
        }

        public static ServiceResult<T> CreateBadRequest<T>(string message)
        {
            return ServiceResult<T>.FromError(new ProblemDetails
            {
                Detail = message,
                Instance = "api",
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad Request",
            });
        }

        public static ServiceResult<T> CreateValiationErrors<T>(EvaluationResults evaluationResults)
        {
            return ServiceResult<T>.FromError(new ProblemDetails
            {
                Detail = "Validation error",
                Instance = "api",
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation error",
                //Extensions = evaluationResults.Errors!.ToDictionary(x => x.Key, x => (object?)x.Value)
            });
        }
    }
}
