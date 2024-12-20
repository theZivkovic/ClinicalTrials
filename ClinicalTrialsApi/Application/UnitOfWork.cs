namespace ClinicalTrialsApi.Application
{
    public interface IUnitOfWork<T>
    {
        public Task Execute(Func<Task> action);
    }
    public class UnitOfWork<T>(ClinicalTrialsContext dbContext) : IUnitOfWork<T>
    {
        public async Task Execute(Func<Task> action)
        {
            try
            {
                using var transaction = dbContext.Database.BeginTransaction();
                await action();
                await dbContext.SaveChangesAsync();
                transaction.Commit();
            }
            finally
            {
                
            }

        }
    }
}
