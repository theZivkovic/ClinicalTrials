using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using System.Data;

namespace ClinicalTrialsApi.Core.Interfaces
{
    public interface IClinicalTrialMetadataRepository
    {
        public Task<ClinicalTrialMetadata> CreateOrUpdate(ClinicalTrialMetadata request);
        public Task<Option<ClinicalTrialMetadata>> Get(string trialId);
        public Task<IEnumerable<ClinicalTrialMetadata>> GetAll(ClinicalTrialsFilter filter);
    }
}
