using ClinicalTrialsApi.Application.Extensions;
using ClinicalTrialsApi.Application.Factories;
using ClinicalTrialsApi.Core.Converters;
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
                return ServiceResult<ClinicalTrialMetadata>.FromError(
                    ProblemDetailsFactory.CreateBadRequest("Only *.json files are allowed"));
            }

            if (file.Length > 97+255+50)
            {
                return ServiceResult<ClinicalTrialMetadata>.FromError(
                    ProblemDetailsFactory.CreateBadRequest("Max file size is 402 bytes"));
            }

            var validationSchema = await validationSchemaRepository.Get(ValidationSchemaType.ClinicalTrial);

            var inputJson = await file.ToJson();

            return await validationSchema.MatchAsync(async validationSchema =>
            {
                var schema = JsonSchema.FromText(validationSchema.Schema.ToString());

                var validationResult = schema.Evaluate(inputJson, new EvaluationOptions
                {
                    RequireFormatValidation = true,
                    OutputFormat = OutputFormat.List
                });

                if (!validationResult.IsValid)
                {
                    return ServiceResult<ClinicalTrialMetadata>.FromError(
                        ProblemDetailsFactory.CreateValidationErrors(validationResult));
                }

                return await unitOfWork.Execute(() =>
                {
                    JsonSerializerOptions options = new()
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    options.Converters.Add(new JsonEnumMemberStringEnumConverter());

                    var clinicalTrialMetadata = JsonSerializer.Deserialize<ClinicalTrialMetadata>(inputJson, options) 
                        ?? throw new JsonException("Problem deserializing the result");
                    
                    clinicalTrialMetadata.AdjustEndDate();
                    return clinicalTrialMetadataRepository.CreateOrUpdate(clinicalTrialMetadata);
                });
            }, () => ServiceResult<ClinicalTrialMetadata>.FromError(
                ProblemDetailsFactory.CreateNotFound($"ValidationSchema for type: {ValidationSchemaType.ClinicalTrial} not found")));
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
               () => ServiceResult<ClinicalTrialMetadata>.FromError(
                   ProblemDetailsFactory.CreateNotFound($"Trial with id: {trialId} not found")));
        }
    }
}
