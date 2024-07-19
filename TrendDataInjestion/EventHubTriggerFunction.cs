using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TrendEventData;


namespace TrendDataIngestion
{
    public class EventHubTriggerFunction
    {
        private static readonly string connectionString = Environment.GetEnvironmentVariable("TrendEventHubConnectionString");
        private static readonly string eventHubName = Environment.GetEnvironmentVariable("TrendEventHubName");
        private static readonly string tableconnectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");

        private readonly ITrendDataRepository _trendDataRepository;
 
        private readonly ILogger<EventHubTriggerFunction> _logger;

        public EventHubTriggerFunction(ITrendDataRepository trendDataRepository, ILogger<EventHubTriggerFunction> logger)
        {
            _trendDataRepository = trendDataRepository;
            _logger = logger;
        }

       

        public class TrendDataItem
        {
            public string Tag { get; set; }
            public double V { get; set; }
            public long T { get; set; }
        }

        public class TrendDataEvent
        {
            public string DeviceId { get; set; }
            
        }


        [Function(nameof(EventHubTriggerFunction))]
        public async Task RunAsync(
            [EventHubTrigger("%TrendEventHubName%", Connection = "TrendEventHubConnectionString")] string[] events,
            FunctionContext context)
        {
            var logger = context.GetLogger<EventHubTriggerFunction>();

            foreach (string eventData in events)
            {
                try
                {

                    var trendDataEvent = JsonConvert.DeserializeObject<dynamic>(eventData);
                    logger.LogInformation($"Received json message: {trendDataEvent}");

                    string deviceId = trendDataEvent.d;
                   
                   
                    long tagId = 0;
                    foreach (var item in trendDataEvent.trend)
                    {
                        long time = item.T;
                        string deviceprofileId = Guid.NewGuid().ToString();
                        var eventDataEntity = new TrendDataTableEntity(deviceId, time)
                        {
                            DeviceId = deviceId,
                            TagId = item.V,
                            DeviceTimeStamp = item.T,
                            DeviceProfileId = deviceprofileId, // Assuming DeviceProfileId is the same as DeviceId
                            Status = true // Example of setting Status
                        };
                        await _trendDataRepository.TrendDataInsertionAsync(eventDataEntity);
                        logger.LogInformation($" DeviceId: {eventDataEntity.DeviceId} details Successfully Stored in Azure Table Storage");
                    }
                }

                catch (Exception e)
                {
                    logger.LogError($"Error processing event: {e.Message}");

                }

            }
        }
          
        }
    }



