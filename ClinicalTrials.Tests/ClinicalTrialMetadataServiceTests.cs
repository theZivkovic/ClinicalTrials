
using ClinicalTrials.Tests.Helpers;
using ClinicalTrials.Tests.MockModels;
using ClinicalTrialsApi.Application;
using ClinicalTrialsApi.Application.Services;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        [TestCase("test.txt")]
        [TestCase("test.doc")]
        public async Task CreateOrUpdate_Should_Not_Allow_Non_Json_Files(string filename)
        {
            var file = FormFileHelper.CreateMockFile(filename, @"{
                ""trialId"": ""trial-2"",
                ""title"": ""Second trial"",
                ""startDate"": ""2016-06-03T23:15:33.008Z"",
                ""participants"": 1,
                ""status"":""Completed""
            }");

            var result = await service.CreateOrUpdateATrial(file);
            Assert.IsTrue(result.IsError);
            Assert.That(result.Error.ValueUnsafe().Status, Is.EqualTo(400));
            Assert.That(result.Error.ValueUnsafe().Title, Is.EqualTo("Bad Request"));
            Assert.That(result.Error.ValueUnsafe().Detail, Is.EqualTo("Only *.json files are allowed"));
        }

        [Test]
        public async Task CreateOrUpdate_Should_Not_Allow_Too_Large_Files()
        {
            var file = FormFileHelper.CreateMockFile("test.json", $@"{{
                ""trialId"": ""{string.Join("", Enumerable.Range(0, 100))}"",
                ""title"": ""{string.Join("", Enumerable.Range(0, 300))}"",
                ""startDate"": ""2016-06-03T23:15:33.008Z"",
                ""participants"": 1,
                ""status"":""Completed""
            }}");

            var result = await service.CreateOrUpdateATrial(file);
            Assert.IsTrue(result.IsError);
            Assert.That(result.Error.ValueUnsafe().Status, Is.EqualTo(400));
            Assert.That(result.Error.ValueUnsafe().Title, Is.EqualTo("Bad Request"));
            Assert.That(result.Error.ValueUnsafe().Detail, Is.EqualTo("Max file size is 402 bytes"));
        }
    }
}