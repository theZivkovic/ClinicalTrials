using ClinicalTrialsApi;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<MigrationService>();
        services.AddDbContextPool<ClinicalTrialsContext>(opt =>
            opt.UseNpgsql("Host=clinicaltrialsdb;Database=postgres;Username=postgres;Password=postgres"));
    })
    .Build();

var my = host.Services.GetRequiredService<MigrationService>();
await my.ExecuteAsync();

class MigrationService
{
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(ILogger<MigrationService> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken = default)
    {
        using (var context = (ClinicalTrialsContext)host.Services.GetService(typeof(ClinicalTrialsContext)))
        {
            context.Database.Migrate();
        }
        Console.WriteLine("Done");
    }
}