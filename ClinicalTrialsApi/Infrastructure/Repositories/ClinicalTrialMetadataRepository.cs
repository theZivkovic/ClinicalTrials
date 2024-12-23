using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ClinicalTrialsApi.Infrastructure.Repositories
{
    public class ClinicalTrialMetadataRepository(ClinicalTrialsContext dbContext) : IClinicalTrialMetadataRepository
    {
        public async Task<Option<ClinicalTrialMetadata>> Get(string trialId)
        {
            var result = await dbContext.ClinicalTrialMetadatas.FirstOrDefaultAsync(x => x.TrialId == trialId);
            return result == null
                ? Option<ClinicalTrialMetadata>.None
                : Option<ClinicalTrialMetadata>.Some(result);
        }

        public Task<IEnumerable<ClinicalTrialMetadata>> GetAll(ClinicalTrialsFilter filter)
        {
            throw new NotImplementedException();
        }

        public async Task<ClinicalTrialMetadata> CreateOrUpdate(ClinicalTrialMetadata request)
        {
            var existingMetadata = await dbContext.ClinicalTrialMetadatas.FirstOrDefaultAsync(x => x.TrialId == request.TrialId);
            if (existingMetadata == null)
            {
                dbContext.Add(request);
            }
            else
            {
                dbContext.Entry(existingMetadata).CurrentValues.SetValues(request);
            }
            return request;
        }

       
    }
}
