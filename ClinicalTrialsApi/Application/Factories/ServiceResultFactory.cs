using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ClinicalTrialsApi.Application.Factories
{
    public static class ServiceResultFactory
    {
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
                Title = "Not Found",
            });
        }
    }
}
