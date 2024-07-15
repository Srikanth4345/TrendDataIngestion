using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

public class TableStorageRepository : ITableStorageRepository
{
    private readonly CloudTable _cloudTable;
    private static readonly string connectionString = Environment.GetEnvironmentVariable("TrendEventHubConnectionString");
    private static readonly string eventHubName = Environment.GetEnvironmentVariable("TrendEventHubName");
    private static readonly string tableconnectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");

    public TableStorageRepository(string connectionString, string tableName)
    {
        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        _cloudTable = tableClient.GetTableReference(tableName);
    }

    public async Task StoreEntityAsync(TrendDataTableEntity entity)
    {
        var operation = TableOperation.InsertOrMerge(entity);
        await _cloudTable.ExecuteAsync(operation);
    }
}
