using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TrendDataInjestion
{
    public class EventHubTriggerFunction
    {
        private static readonly string connectionString = Environment.GetEnvironmentVariable("TrendEventHubConnectionString");
        private static readonly string eventHubName = Environment.GetEnvironmentVariable("TrendEventHubName");
        private static readonly string tableconnectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
        public class MyEventDataClass // Define a class to represent your event data
        {
            public string DeviceId { get; set; }
            public double Temperature { get; set; }
            public double Humidity { get; set; }

            public long TimeStamp {  get; set; }
            // Add other properties as needed
        }


        [Function(nameof(EventHubTriggerFunction))]
        public static async Task Run(
            [EventHubTrigger("%TrendEventHubName%", Connection = "TrendEventHubConnectionString")] string[] events,
            FunctionContext context)
        {
            var logger = context.GetLogger<EventHubTriggerFunction>();
            var eventHubClient = new EventHubProducerClient(connectionString, eventHubName);

            List<EventData> eventDataList = new List<EventData>();

            var tableClient = CloudStorageAccount.Parse(tableconnectionString).CreateCloudTableClient(new TableClientConfiguration());

            var table = tableClient.GetTableReference("TrendDataTable");
            foreach (string eventData in events)
            {
                try
                {
                    logger.LogInformation($"Received message: {eventData}");

                    var jsonMessage = JsonConvert.DeserializeObject<dynamic>(eventData);
                   

                    // Extract DeviceId
                    string deviceId = jsonMessage.d;

                    // Extract Temperature and Humidity from "trend" array
                    double temperature = 0;
                    double humidity = 0;
                    long time = 0;

                    foreach (var item in jsonMessage.trend)
                    {
                        if (item.Tag == "Temp")
                        {
                            temperature = (double)item.V;
                            time=(long)item.T;

                        }
                        else if (item.Tag == "Humidity")
                        {
                            humidity = (double)item.V;
                            time = (long)item.T;
                        }
                    }


                    var eventDataEntity = new TrendDataTableEntity(deviceId, time.ToString())
                    {
                        //PartitionKey = deviceId,
                        //RowKey =time.ToString(),
                        DeviceId = deviceId,
                        Temperature = temperature,
                        Humidity = humidity,
                        TimeStamp = time
                    };


                    var insertOperation = TableOperation.Insert(eventDataEntity);
                    await table.ExecuteAsync(insertOperation);

                    logger.LogInformation($"Stored in Azure Table Storage: DeviceId: {eventDataEntity.DeviceId}," +
                                          $" Temperature: {eventDataEntity.Temperature}," +
                                          $" Humidity: {eventDataEntity.Humidity}, " +
                                          $"TimeStamp:{eventDataEntity.TimeStamp}");

                   

                    // Example: Sending to another event hub
                    await eventHubClient.SendAsync(eventDataList);

                   

                }
                catch (Exception e)
                {
                    logger.LogError($"Error processing event: {e.Message}");
                   
                }
            }

            await eventHubClient.CloseAsync();
        }
    }
}


