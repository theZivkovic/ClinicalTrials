using ClinicalTrialsApi;
using ClinicalTrialsApi.Application;
using ClinicalTrialsApi.Application.Services;
using ClinicalTrialsApi.Core.Interfaces;
using ClinicalTrialsApi.Core.Models;
using ClinicalTrialsApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<ClinicalTrialsContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("ClinicalTrialsContext"),
        o => o
            .SetPostgresVersion(17, 0)
            .MapEnum<ClinicalTrialStatus>("clinical_trials_status")));

builder.Services.AddScoped<IClinicalTrialMetadataRepository, ClinicalTrialMetadataRepository>();
builder.Services.AddScoped<IValidationSchemaRepository, ValidationSchemaRepository>();
builder.Services.AddScoped<IClinicalTrialMetadataService, ClinicalTrialMetadataService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
