using LanguageExt;
using System.Net;

namespace ClinicalTrialsApi.Application
{
    public class Error
    {
        public string Message { get; set; } = null!;
        public HttpStatusCode HttpStatusCode { get; set; }
    }

    public class ServiceResult<TEntity>
    {
        public Option<TEntity> Entity { get; private set; }
        public Option<Error> Error { get; private set; }
        public bool IsError { get; private set; }

        public static ServiceResult<TEntity> FromEntity(TEntity entity)
        {
            return new ServiceResult<TEntity>
            {
                Entity = Option<TEntity>.Some(entity),
                Error = Option<Error>.None,
                IsError = false
            };
        }

        public static ServiceResult<TEntity> FromError(Error error)
        {
            return new ServiceResult<TEntity>
            {
                Entity = Option<TEntity>.None,
                Error = Option<Error>.Some(error),
                IsError = true
            };
        }
    }
}
