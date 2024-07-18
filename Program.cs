using Microsoft.Azure.Cosmos.Table;
using System;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=saiotproject;AccountKey=TM+Uik26WtaWUFlfIMLN24hu3PA573kZyGiUbtWCMebHgW/IoFhfOgrfJWESDGDyvvxHLdTFTdOI+AStMc9vbw==;EndpointSuffix=core.windows.net";

       
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
        
        CloudTable table = tableClient.GetTableReference("DeviceTable"); 
        
        await table.CreateAsync();
        Console.WriteLine($"Table '{table.Name}'");

        DeviceEntity entity = new DeviceEntity("deviceId1", "deviceName1")
        {
            DeviceId = "90442f83-fba5-4e84-9b73-a29f96d957ac",
            DeviceName = "RaspberryPi",
            DeviceProfileId = "415bfb79-6ce8-4ab3-b36b-07977ed780ef"
        };

        TableOperation insertOperation = TableOperation.Insert(entity);
        await table.ExecuteAsync(insertOperation);

        Console.WriteLine($"Entity inserted into '{table.Name}': DeviceId={entity.DeviceId}, DeviceName={entity.DeviceName}, DeviceProfileId={entity.DeviceProfileId}");
    }
}
