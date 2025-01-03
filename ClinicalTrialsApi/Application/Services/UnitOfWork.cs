﻿using ClinicalTrialsApi.Application.Factories;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialsApi.Application.Services
{
    public interface IUnitOfWork
    {
        public Task<ServiceResult<T>> Execute<T>(Func<Task<T>> action);
    }
    public class UnitOfWork(ClinicalTrialsContext dbContext, ILogger<UnitOfWork> logger) : IUnitOfWork
    {
        public async Task<ServiceResult<T>> Execute<T>(Func<Task<T>> action)
        {
            try
            {
                using var transaction = dbContext.Database.BeginTransaction();
                var result = await action();
                await dbContext.SaveChangesAsync();
                transaction.Commit();
                return ServiceResult<T>.FromEntity(result);
            }
            catch (Exception e)
            {
                logger.LogError("UnitOfWork unhandled exception: {e}", e);
                return ServiceResult<T>.FromError(
                    Application.Factories.ProblemDetailsFactory.CreateInternalServerError(e.Message));
            }
        }
    }
}
