using Microsoft.Azure.Cosmos.Table;

public class DeviceEntity : TableEntity
{
    public DeviceEntity(string deviceId, string deviceName)
    {
        PartitionKey = deviceId;
        RowKey = deviceName;
    }

    public DeviceEntity() { }

    public string DeviceId { get; set; }
    public string DeviceName { get; set; }
    public string DeviceProfileId { get; set; }
}
