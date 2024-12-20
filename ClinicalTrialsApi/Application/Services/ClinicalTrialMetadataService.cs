using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using System.Text.Json;

namespace ClinicalTrialsApi.Application.Services
{
    public interface IClinicalTrialMetadataService
    {
        ServiceResult<ClinicalTrialMetadata> Create(JsonElement request);
    }

    public class ClinicalTrialMetadataService : IClinicalTrialMetadataService
    {
        public ServiceResult<ClinicalTrialMetadata> Create(JsonElement request)
        {
            throw new NotImplementedException();
        }
    }
}
