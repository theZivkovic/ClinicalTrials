using ClinicalTrialsApi.Application.Factories;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using Json.Schema;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using System.Text.Json;

namespace ClinicalTrialsApi.Application.Services
{
    public interface IClinicalTrialMetadataService
    {
        Task<ServiceResult<ClinicalTrialMetadata>> Create(JsonElement request);
    }

    public class ClinicalTrialMetadataService(
        IValidationSchemaRepository validationSchemaRepository,
        IClinicalTrialMetadataRepository clinicalTrialMetadataRepository,
        IUnitOfWork unitOfWork
    ) : IClinicalTrialMetadataService
    {
        public async Task<ServiceResult<ClinicalTrialMetadata>> Create(JsonElement request)
        {
            var validationSchema = await validationSchemaRepository.Get(ValidationSchemaType.ClinicalTrial);

            if (validationSchema.IsNone)
            {
                return ServiceResultFactory.CreateNotFound<ClinicalTrialMetadata>($"ValidationSchema for type: {ValidationSchemaType.ClinicalTrial} not found");
            }

            var schema = JsonSchema.FromText(validationSchema.ValueUnsafe().Schema.ToString());
            var validationResult = schema.Evaluate(request);

            // validate schema and create metadata object
            if (!validationResult.IsValid)
            {
                return ServiceResultFactory.CreateBadRequest<ClinicalTrialMetadata>("Validation error");
            }


            var result = await unitOfWork.Execute<ClinicalTrialMetadata>(() =>
            {
                var clinicalTrialMetadata = JsonSerializer.Deserialize<ClinicalTrialMetadata>(request, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return clinicalTrialMetadataRepository.Update(clinicalTrialMetadata);
            });

            return ServiceResult<ClinicalTrialMetadata>.FromEntity(result);
        }
    }
}
