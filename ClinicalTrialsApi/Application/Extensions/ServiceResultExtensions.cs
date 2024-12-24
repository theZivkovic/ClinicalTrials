using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ClinicalTrialsApi.Application.Extensions
{
    public static class ServiceResultExtensions
    {
        public static IActionResult ToResponse<T>(this ServiceResult<T> result)
        {
            if (!result.IsError)
            {
                return new OkObjectResult(result.Entity.ValueUnsafe());
            }
            else
            {
                return result.Error.ValueUnsafe().Status switch
                {
                    (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(result.Error),
                    (int)HttpStatusCode.NotFound => new NotFoundObjectResult(result.Error),
                    (int)HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(result.Error),
                    _ => new ObjectResult(result.Error)
                };
            }
        }
    }
}
