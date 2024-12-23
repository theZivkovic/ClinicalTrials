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
    }
}
