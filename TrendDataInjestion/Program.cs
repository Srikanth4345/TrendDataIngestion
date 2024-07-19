using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrendEventData.TrendEventTableEntity;
using TrendEventData;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {

        services.AddScoped<ITrendDataRepository>(provider =>
        {
            var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
            var tableName = "TrendDataTable";
            return new TrendDataRepository(connectionString, tableName);
        });

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
    })
    .Build();

host.Run();
