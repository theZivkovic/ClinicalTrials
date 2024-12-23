using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using System.Data;

namespace ClinicalTrialsApi.Infrastructure.Repositories
{
    public class ClinicalTrialMetadataRepository(ClinicalTrialsContext dbContext) : IClinicalTrialMetadataRepository
    {
        public Task<Option<ClinicalTrialMetadata>> Get(string trialId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ClinicalTrialMetadata>> GetAll(ClinicalTrialsFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<ClinicalTrialMetadata> CreateOrUpdate(ClinicalTrialMetadata request)
        {
            var existingMetadata = dbContext.ClinicalTrialMetadatas.FirstOrDefault(x => x.TrialId == request.TrialId);
            if (existingMetadata == null)
            {
                dbContext.Add(request);
            }
            else
            {
                dbContext.Entry(existingMetadata).CurrentValues.SetValues(request);
            }
            return Task.FromResult(request);
        }

       
    }
}
