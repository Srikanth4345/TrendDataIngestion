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
       
       
        public TrendDataRepository(string connectionString, string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _cloudTable = tableClient.GetTableReference(tableName);
        }
        
        public async Task TrendDataInsertionAsync(TrendDataTableEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var operation = TableOperation.Insert(entity);
            await _cloudTable.ExecuteAsync(operation);
        }

       

       
    }

}
