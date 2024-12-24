
using ClinicalTrials.Tests.Helpers;
using ClinicalTrials.Tests.MockModels;
using ClinicalTrialsApi.Application.Services;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using LanguageExt;
using LanguageExt.UnitTesting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace ClinicalTrials.Tests
{
    public class Tests
    {
        ClinicalTrialMetadataService service;
        Mock<IClinicalTrialMetadataRepository> clinicalTrialMetadataRepository;
        Mock<IValidationSchemaRepository> validationSchemaRepository;

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

            clinicalTrialMetadataRepository = new Mock<IClinicalTrialMetadataRepository>();
            clinicalTrialMetadataRepository.Setup(x => x.CreateOrUpdate(It.IsAny<ClinicalTrialMetadata>()))
                .ReturnsAsync((ClinicalTrialMetadata request) => request);

            validationSchemaRepository = new Mock<IValidationSchemaRepository>();
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
        public async Task CreateOrUpdateATrial_Should_Not_Allow_Non_Json_Files(string filename)
        {
            var file = FormFileHelper.CreateMockFile(filename, @"{
                ""trialId"": ""trial-2"",
                ""title"": ""Second trial"",
                ""startDate"": ""2016-06-03T23:15:33.008Z"",
                ""participants"": 1,
                ""status"":""Completed""
            }");

            var result = await service.CreateOrUpdateATrial(file);
            Assert.That(result.IsError, Is.True);
            result.Error.ShouldBeSome(x => Assert.That(x.Status, Is.EqualTo(400)));
            result.Error.ShouldBeSome(x => Assert.That(x.Title, Is.EqualTo("Bad Request")));
            result.Error.ShouldBeSome(x => Assert.That(x.Detail, Is.EqualTo("Only *.json files are allowed")));
        }

        [Test]
        public async Task CreateOrUpdateATrial_Should_Not_Allow_Too_Large_Files()
        {
            var file = FormFileHelper.CreateMockFile("test.json", $@"{{
                ""trialId"": ""{string.Join("", Enumerable.Range(0, 100))}"",
                ""title"": ""{string.Join("", Enumerable.Range(0, 300))}"",
                ""startDate"": ""2016-06-03T23:15:33.008Z"",
                ""participants"": 1,
                ""status"":""Completed""
            }}");

            var result = await service.CreateOrUpdateATrial(file);
            Assert.That(result.IsError, Is.True);
            result.Error.ShouldBeSome(x => Assert.That(x.Status, Is.EqualTo(400)));
            result.Error.ShouldBeSome(x => Assert.That(x.Title, Is.EqualTo("Bad Request")));
            result.Error.ShouldBeSome(x => Assert.That(x.Detail, Is.EqualTo("Max file size is 402 bytes")));
        }

        [Test]
        public async Task CreateOrUpdateATrial_Should_Return_Error_If_ValidationSchema_Is_Not_Available()
        {
            validationSchemaRepository
                .Setup(x => x.Get(It.IsAny<ValidationSchemaType>()))
                .ReturnsAsync(Option<ValidationSchema>.None);

            var file = FormFileHelper.CreateMockFile("test.json", @"{
                ""trialId"": ""trial-2"",
                ""title"": ""Second trial"",
                ""startDate"": ""2016-06-03T23:15:33.008Z"",
                ""participants"": 1,
                ""status"":""Completed""
            }");

            var result = await service.CreateOrUpdateATrial(file);
            Assert.That(result.IsError, Is.True);
            result.Error.ShouldBeSome(x => Assert.That(x.Status, Is.EqualTo(404)));
            result.Error.ShouldBeSome(x => Assert.That(x.Title, Is.EqualTo("Not Found")));
            result.Error.ShouldBeSome(x => Assert.That(x.Detail, Is.EqualTo($"ValidationSchema for type: {ValidationSchemaType.ClinicalTrial} not found")));
        }

        [TestCase(@"{
                ""title"": ""Second trial"",
                ""startDate"": ""2016-06-03T23:15:33.008Z"",
                ""participants"": 1,
                ""status"":""Completed""
            }", "")]
        [TestCase(@"{
                ""trialId"": ""trial-2"",
                ""title"": ""Second trial"",
                ""startDate"": ""2022-AA-BB"",
                ""participants"": 1,
                ""status"":""Completed""
            }", "startDate")]
        [TestCase(@"{
                ""trialId"": ""trial-2"",
                ""title"": ""Second trial"",
                ""startDate"": ""2022-12-12"",
                ""participants"": 1,
                ""status"":""SOME STATUS""
            }", "status")]
        public async Task CreateOrUpdateATrial_Should_Return_Validation_Error_If_Request_Doesnt_Match_The_Schema(string request, string corruptedFieldName)
        {
            var file = FormFileHelper.CreateMockFile("test.json", request);

            var result = await service.CreateOrUpdateATrial(file);
            Assert.That(result.IsError, Is.True);
            result.Error.ShouldBeSome(x => Assert.That(x.Status, Is.EqualTo(400)));
            result.Error.ShouldBeSome(x => Assert.That(x.Title, Is.EqualTo("Validation Error")));
            result.Error.ShouldBeSome(x => Assert.That(x.Extensions["errors"], Contains.Key(corruptedFieldName)));
        }
    }
}