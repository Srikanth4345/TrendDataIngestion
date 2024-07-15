using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TrendEventData;
using static Grpc.Core.Metadata;

namespace TrendDataInjestion
{
    public class EventHubTriggerFunction
    {
        private static readonly string connectionString = Environment.GetEnvironmentVariable("TrendEventHubConnectionString");
        private static readonly string eventHubName = Environment.GetEnvironmentVariable("TrendEventHubName");
        private static readonly string tableconnectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");

        private readonly ITrendDataRepository _trendDataRepository;
        private readonly ITableStorageRepository _tableStorageRepository;
        private readonly ILogger<EventHubTriggerFunction> _logger;

        public EventHubTriggerFunction(ITrendDataRepository trendDataRepository, ITableStorageRepository tableStorageRepository, ILogger<EventHubTriggerFunction> logger)
        {
            _trendDataRepository = trendDataRepository;
            _tableStorageRepository = tableStorageRepository;
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
                    long time = 0;
                    long tagId = 0;
                    foreach (var item in trendDataEvent.trend)
                    {

                      
                       string deviceprofileId =  Guid.NewGuid().ToString();
                        var eventDataEntity = new TrendDataTableEntity(trendDataEvent.d, time)
                        {
                            DeviceId = deviceId,
                            TagId = item.V,
                            Timestamp = item.T,
                            DeviceProfileId = deviceprofileId, // Assuming DeviceProfileId is the same as DeviceId
                            Status = true // Example of setting Status
                        };
                        await _tableStorageRepository.StoreEntityAsync(eventDataEntity);
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


