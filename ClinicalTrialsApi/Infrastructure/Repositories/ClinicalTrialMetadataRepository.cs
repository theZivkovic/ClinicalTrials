using ClinicalTrialsApi.Core.DTOs;
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

        public async Task<IEnumerable<ClinicalTrialMetadata>> GetAll(ClinicalTrialMetadataFilter filter, Pagination pagination)
        {
            var filteredTrials = dbContext.ClinicalTrialMetadatas.AsQueryable();

            if (filter.TrialIds.Any())
            {
                filteredTrials = filteredTrials.Where(trial => filter.TrialIds.Any(trialId => trialId == trial.TrialId));
            }

            if (filter.Statuses.Any())
            {
                filteredTrials = filteredTrials.Where(trial => filter.Statuses.Any(status => status == trial.Status));
            }

            return await filteredTrials
                .OrderByDescending(x => x.StartDate)
                .Skip(pagination.Offset)
                .Take(pagination.Limit)
                .ToListAsync();

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
