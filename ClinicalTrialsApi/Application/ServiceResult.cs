using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ClinicalTrialsApi.Application
{
    public class ServiceResult<TEntity>
    {
        public Option<TEntity> Entity { get; private set; }
        public Option<ProblemDetails> Error { get; private set; }
        public bool IsError { get; private set; }

        public static ServiceResult<TEntity> FromEntity(TEntity entity)
        {
            return new ServiceResult<TEntity>
            {
                Entity = Option<TEntity>.Some(entity),
                Error = Option<ProblemDetails>.None,
                IsError = false
            };
        }

        public static ServiceResult<TEntity> FromError(ProblemDetails error)
        {
            return new ServiceResult<TEntity>
            {
                Entity = Option<TEntity>.None,
                Error = Option<ProblemDetails>.Some(error),
                IsError = true
            };
        }
    }
}
