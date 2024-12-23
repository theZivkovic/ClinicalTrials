using ClinicalTrialsApi.Application.Factories;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using Json.Schema;
using LanguageExt.UnsafeValueAccess;
using System.Text.Json;

namespace ClinicalTrialsApi.Application.Services
{
    public interface IClinicalTrialMetadataService
    {
        Task<ServiceResult<ClinicalTrialMetadata>> CreateOrUpdateATrial(JsonElement request);
        Task<ServiceResult<ClinicalTrialMetadata>> GetATrial(string trialId);
    }

    public class ClinicalTrialMetadataService(
        IValidationSchemaRepository validationSchemaRepository,
        IClinicalTrialMetadataRepository clinicalTrialMetadataRepository,
        IUnitOfWork unitOfWork
    ) : IClinicalTrialMetadataService
    {
        public async Task<ServiceResult<ClinicalTrialMetadata>> CreateOrUpdateATrial(JsonElement request)
        {
            var validationSchema = await validationSchemaRepository.Get(ValidationSchemaType.ClinicalTrial);

            if (validationSchema.IsNone)
            {
                return ServiceResultFactory.CreateNotFound<ClinicalTrialMetadata>($"ValidationSchema for type: {ValidationSchemaType.ClinicalTrial} not found");
            }

            var schema = JsonSchema.FromText(validationSchema.ValueUnsafe().Schema.ToString());
            var validationResult = schema.Evaluate(request);

            if (!validationResult.IsValid)
            {
                return ServiceResultFactory.CreateValiationErrors<ClinicalTrialMetadata>(validationResult);
            }

            return await unitOfWork.Execute(() =>
            {
                var clinicalTrialMetadata = JsonSerializer.Deserialize<ClinicalTrialMetadata>(request, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return clinicalTrialMetadata == null
                    ? throw new JsonException("Problem deserializing the result")
                    : clinicalTrialMetadataRepository.CreateOrUpdate(clinicalTrialMetadata);
            });

        }

        public async Task<ServiceResult<ClinicalTrialMetadata>> GetATrial(string trialId)
        {
            var result = await clinicalTrialMetadataRepository.Get(trialId);

            return result.Match(
               ServiceResult<ClinicalTrialMetadata>.FromEntity,
               () => ServiceResultFactory.CreateNotFound<ClinicalTrialMetadata>($"Trial with id: {trialId} not found"));
        }
    }
}
