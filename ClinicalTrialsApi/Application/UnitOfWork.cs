namespace ClinicalTrialsApi.Application
{
    public interface IUnitOfWork
    {
        public Task<T> Execute<T>(Func<Task<T>> action);
    }
    public class UnitOfWork(ClinicalTrialsContext dbContext) : IUnitOfWork
    {
        public async Task<T> Execute<T>(Func<Task<T>> action)
        {
            try
            {
                using var transaction = dbContext.Database.BeginTransaction();
                var result = await action();
                await dbContext.SaveChangesAsync();
                transaction.Commit();
                return result;
            }
            finally
            {
                
            }

        }
    }
}
