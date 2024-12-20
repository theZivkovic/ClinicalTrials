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

        public Task<ClinicalTrialMetadata> Update(ClinicalTrialMetadata request)
        {
            dbContext.Update(request);
            return Task.FromResult(request);
        }

       
    }
}
