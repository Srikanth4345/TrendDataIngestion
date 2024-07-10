using Microsoft.Azure.Cosmos.Table;
using System.Collections.Concurrent;

public class TrendDataTableEntity : TableEntity
{
    public string DeviceId { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public long TimeStamp { get; set; }

    public TrendDataTableEntity(string partitionKey, string rowKey)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
    }

    public TrendDataTableEntity() { } // Parameterless constructor required for deserialization
}
