using ClinicalTrialsApi;
using ClinicalTrialsApi.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContextPool<ClinicalTrialsContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("ClinicalTrialsContext"),
        o => o
            .SetPostgresVersion(17, 0)
            .MapEnum<ClinicalTrialStatus>("clinical_trials_status"))
    .UseSeeding((context, _) =>
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

        var existingSchema = context
            .Set<ValidationSchema>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Type == ValidationSchemaType.ClinicalTrial);

        if (existingSchema == null)
        {
            context.Set<ValidationSchema>().Add(new ValidationSchema
            {
                Type = ValidationSchemaType.ClinicalTrial,
                Schema = JsonDocument.Parse(clinicalTrialSchema).RootElement
            });
            context.SaveChanges();
        }
    }));

var app = builder.Build();

using (var Scope = app.Services.CreateScope())
{
    var context = Scope.ServiceProvider.GetRequiredService<ClinicalTrialsContext>();
    context.Database.Migrate();
}

app.Run();

