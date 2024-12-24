using ClinicalTrialsApi.Application.Factories;
using ClinicalTrialsApi.Core.DTOs;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using Json.Schema;
using LanguageExt.UnsafeValueAccess;
using System.Text.Json;

namespace ClinicalTrialsApi.Application.Services
{
    public interface IClinicalTrialMetadataService
    {
        Task<ServiceResult<ClinicalTrialMetadata>> CreateOrUpdateATrial(IFormFile file);
        Task<ServiceResult<ClinicalTrialMetadata>> GetATrial(string trialId);
        Task<ServiceResult<IEnumerable<ClinicalTrialMetadata>>> GetAllTrials(ClinicalTrialMetadataFilter filter, Pagination pagination);
    }

    public class ClinicalTrialMetadataService(
        IValidationSchemaRepository validationSchemaRepository,
        IClinicalTrialMetadataRepository clinicalTrialMetadataRepository,
        IUnitOfWork unitOfWork
    ) : IClinicalTrialMetadataService
    {
        public async Task<ServiceResult<ClinicalTrialMetadata>> CreateOrUpdateATrial(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            
            if (extension != ".json")
            {
                return ServiceResultFactory.CreateBadRequest<ClinicalTrialMetadata>("Only *.json files are allowed");
            }

            if (file.Length > 97+255+50)
            {
                return ServiceResultFactory.CreateBadRequest<ClinicalTrialMetadata>("Max file size is 402 bytes");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var fileContent = await reader.ReadToEndAsync();

            var inputJson = JsonDocument.Parse(fileContent).RootElement;

            var validationSchema = await validationSchemaRepository.Get(ValidationSchemaType.ClinicalTrial);

            if (validationSchema.IsNone)
            {
                return ServiceResultFactory.CreateNotFound<ClinicalTrialMetadata>($"ValidationSchema for type: {ValidationSchemaType.ClinicalTrial} not found");
            }

            var schema = JsonSchema.FromText(validationSchema.ValueUnsafe().Schema.ToString());
            var validationResult = schema.Evaluate(inputJson);

            if (!validationResult.IsValid)
            {
                return ServiceResultFactory.CreateValiationErrors<ClinicalTrialMetadata>(validationResult);
            }

            return await unitOfWork.Execute(() =>
            {
                var clinicalTrialMetadata = JsonSerializer.Deserialize<ClinicalTrialMetadata>(inputJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return clinicalTrialMetadata == null
                    ? throw new JsonException("Problem deserializing the result")
                    : clinicalTrialMetadataRepository.CreateOrUpdate(clinicalTrialMetadata);
            });

        }

        public async Task<ServiceResult<IEnumerable<ClinicalTrialMetadata>>> GetAllTrials(ClinicalTrialMetadataFilter filter, Pagination pagination)
        {
            return ServiceResult<IEnumerable<ClinicalTrialMetadata>>
                .FromEntity(await clinicalTrialMetadataRepository.GetAll(filter, pagination));
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
