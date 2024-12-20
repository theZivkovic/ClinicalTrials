using ClinicalTrialsApi.Application.Factories;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using System.Text.Json;

namespace ClinicalTrialsApi.Application.Services
{
    public interface IClinicalTrialMetadataService
    {
        Task<ServiceResult<ClinicalTrialMetadata>> Create(JsonElement request);
    }

    public class ClinicalTrialMetadataService(
        IValidationSchemaRepository validationSchemaRepository,
        IClinicalTrialMetadataRepository clinicalTrialMetadataRepository
    ) : IClinicalTrialMetadataService
    {
        public async Task<ServiceResult<ClinicalTrialMetadata>> Create(JsonElement request)
        {
            var validationSchema = await validationSchemaRepository.Get(ValidationSchemaType.ClinicalTrial);

            if (validationSchema.IsNone)
            {
                return ServiceResultFactory.CreateNotFound<ClinicalTrialMetadata>($"ValidationSchema for type: {ValidationSchemaType.ClinicalTrial} not found");
            }

            // validate schema and create metadata object

            var result = await clinicalTrialMetadataRepository.Update(new ClinicalTrialMetadata());

            return ServiceResult<ClinicalTrialMetadata>.FromEntity(result);
        }
    }
}
