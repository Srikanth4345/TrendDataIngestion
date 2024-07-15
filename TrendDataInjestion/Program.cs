using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrendEventData.TrendEventTableEntity;
using TrendEventData;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {

        services.AddSingleton<ITrendDataRepository>(provider =>
        {
            var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
            var tableName = "TrendDataTable"; // Replace with your table name
            return new TrendDataRepository(connectionString, tableName);
        });

        // Register ITableStorageRepository with TableStorageRepository implementation
        services.AddSingleton<ITableStorageRepository>(provider =>
        {
            var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
            var tableName = "TrendDataTable"; // Replace with your table name
            return new TableStorageRepository(connectionString, tableName);
        });
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
    })
    .Build();

host.Run();
