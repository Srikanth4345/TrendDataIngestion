using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrendEventData;

namespace TrendEventData.TrendEventTableEntity
{
    public class TrendDataRepository : ITrendDataRepository
    {
        private readonly CloudTable _cloudTable;
        private static readonly string connectionString = Environment.GetEnvironmentVariable("TrendEventHubConnectionString");
        private static readonly string eventHubName = Environment.GetEnvironmentVariable("TrendEventHubName");
        private static readonly string tableconnectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");


        public TrendDataRepository(string connectionString, string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(tableconnectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _cloudTable = tableClient.GetTableReference(tableName);
        }

        public async Task TrendDataInsertionAsync(TrendDataTableEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var operation = TableOperation.InsertOrMerge(entity);
            await _cloudTable.ExecuteAsync(operation);
        }

       

        public async Task<List<TrendDataTableEntity>> TrendDataRetrievalAsync(string partitionKey, DateTime startTime, DateTime endTime)
        {
            var filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                TableOperators.And,
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, startTime),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, endTime)
                )
            );

            var query = new TableQuery<TrendDataTableEntity>().Where(filter);
            var results = new List<TrendDataTableEntity>();

            TableContinuationToken token = null;
            do
            {
                var segment = await _cloudTable.ExecuteQuerySegmentedAsync(query, token);
                results.AddRange(segment.Results);
                token = segment.ContinuationToken;
            } while (token != null);

            return results;
        }
    }

}
