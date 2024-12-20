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
    }
}
