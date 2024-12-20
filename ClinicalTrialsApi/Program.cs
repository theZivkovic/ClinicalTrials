using ClinicalTrialsApi;
using ClinicalTrialsApi.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var Scope = app.Services.CreateScope())
{
    var context = Scope.ServiceProvider.GetRequiredService<ClinicalTrialsContext>();
    context.Database.Migrate();
}

app.Run();
