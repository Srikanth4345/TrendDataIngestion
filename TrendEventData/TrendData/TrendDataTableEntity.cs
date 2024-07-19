using Microsoft.Azure.Cosmos.Table;
using System.Collections.Concurrent;

public class TrendDataTableEntity : TableEntity
{
    public string DeviceId { get; set; }
    public long TagId { get; set; }

    public string DeviceProfileId { get; set; }
    public long DeviceTimeStamp { get; set; }

    public bool Status { get; set; }
    public TrendDataTableEntity(string deviceId, long timeStamp)
    {
        PartitionKey = deviceId;
        RowKey = $"{timeStamp}_{Guid.NewGuid()}"; // Generate a unique RowKey
    }

    public TrendDataTableEntity() { } // Parameterless constructor required for deserialization
   
}
