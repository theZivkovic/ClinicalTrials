
using ClinicalTrials.Tests.MockModels;
using ClinicalTrialsApi.Application;
using ClinicalTrialsApi.Application.Services;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace ClinicalTrials.Tests
{
    public class Tests
    {
        IClinicalTrialMetadataService service;

        [SetUp]
        public void Setup()
        {
            var clinicalTrialSchema = @"{
            ""$schema"": ""http://json-schema.org/draft-07/schema#"",
            ""title"": ""ClinicalTrialMetadata"",
            ""type"": ""object"",
            ""properties"": {
                ""trialId"": {
                    ""type"": ""string""
                },
                ""title"": {
                    ""type"": ""string""
                },
                ""startDate"": {
                    ""type"": ""string"",
                    ""format"": ""date""
                },
                ""endDate"": {
                    ""type"": ""string"",
                    ""format"": ""date""
                },
                ""participants"": {
                    ""type"": ""integer"",
                    ""minimum"": 1
                },
                ""status"": {
                    ""type"": ""string"",
                    ""enum"": [
                        ""Not Started"",
                        ""Ongoing"",
                        ""Completed""
                    ]
                }
            },
            ""required"": [
                ""trialId"",
                ""title"",
                ""startDate"",
                ""status""
            ],
            ""additionalProperties"": false
            }";

            var clinicalTrialMetadataRepository = new Mock<IClinicalTrialMetadataRepository>();
            clinicalTrialMetadataRepository.Setup(x => x.CreateOrUpdate(It.IsAny<ClinicalTrialMetadata>()))
                .ReturnsAsync((ClinicalTrialMetadata request) => request);

            var validationSchemaRepository = new Mock<IValidationSchemaRepository>();
            validationSchemaRepository.Setup(x => x.Get(ValidationSchemaType.ClinicalTrial)).ReturnsAsync(Option<ValidationSchema>.Some(new ValidationSchema
            {
                Type = ValidationSchemaType.ClinicalTrial,
                Schema = JsonDocument.Parse(clinicalTrialSchema).RootElement
            }));
           
            service = new ClinicalTrialMetadataService(
                validationSchemaRepository.Object,
                clinicalTrialMetadataRepository.Object,
                new UnitOfWork(new MockClinicalTrialsContext().Object, new Mock<ILogger<UnitOfWork>>().Object));

        }

        [Test]
        public async Task Test1()
        {
            var content = @"{
                ""trialId"": ""trial-2"",
                ""title"": ""Second trial"",
                ""startDate"": ""2016-06-03T23:15:33.008Z"",
                ""participants"": 1,
                ""status"":""Completed""
            }";
            var fileName = "test.json";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            //create FormFile with desired data
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

            var result = await service.CreateOrUpdateATrial(file);
            Assert.IsFalse(result.IsError);
            
        }
    }
}