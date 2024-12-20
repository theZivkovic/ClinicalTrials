using ClinicalTrialsApi.Core.Models;
using System.Data;

namespace ClinicalTrialsApi.Core.Interfaces
{
    public interface IClinicalTrialMetadataRepository
    {
        public Task<ClinicalTrialMetadata> Update(ClinicalTrialMetadata request);
        public Task<ClinicalTrialMetadata> Get(string trialId);
        public Task<IEnumerable<ClinicalTrialMetadata>> GetAll(ClinicalTrialsFilter filter);
    }
}
