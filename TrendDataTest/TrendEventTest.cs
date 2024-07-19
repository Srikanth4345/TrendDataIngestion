using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.Cosmos.Table;
using Moq;
using System.Threading.Tasks;
using System;
using Google.Protobuf.WellKnownTypes;

namespace TrendEventData.TrendEventTableEntity.Tests
{
    [TestClass]
    public class TrendDataRepositoryTests
    {
        private TrendDataRepository trendDataRepository;
        private Mock<CloudTable> _cloudTable;

        [TestInitialize]
        public void Initialize()
        {
            _cloudTable = new Mock<CloudTable>(new Uri("https://saiotproject.table.core.windows.net/TrendDataTable"));
            _cloudTable.Setup(c => c.ExecuteAsync(It.IsAny<TableOperation>()))
                       .ReturnsAsync(new TableResult { HttpStatusCode = 204 });

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=saiotproject;AccountKey=TM+Uik26WtaWUFlfIMLN24hu3PA573kZyGiUbtWCMebHgW/IoFhfOgrfJWESDGDyvvxHLdTFTdOI+AStMc9vbw==;EndpointSuffix=core.windows.net";
            string tableName = "TrendDataTable"; 

            trendDataRepository = new TrendDataRepository(connectionString, tableName);
        }
        [TestMethod]
        public async Task TrendDataInsertionAsync_ValidEntity_Success()
        {
            // Arrange
            var entity = new TrendDataTableEntity
            {
                PartitionKey = "2486d93f-0c2b-4d72-82f9-585a692149c3",  
                RowKey = Guid.NewGuid().ToString(),  
                DeviceId = "2486d93f-0c2b-4d72-82f9-585a692149c3",
                DeviceProfileId = "29a68176-2a99-41c2-907d-664d37dc491b",
                Status = true,
                TagId = 25,
                TimeStamp = 1721121900 
             
            };

            // Act
            await trendDataRepository.TrendDataInsertionAsync(entity);

            // Assert
            _cloudTable.Verify(
                table => table.ExecuteAsync(It.Is<TableOperation>(op => op.OperationType == TableOperationType.Insert)),
                Times.Once);
        
    }

    [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task TrendDataInsertionAsync_NullEntity_ThrowsException()
        {
            // Arrange
            TrendDataTableEntity entity = null;

            // Act
            await trendDataRepository.TrendDataInsertionAsync(entity);

            // Assert
            // Expects ArgumentNullException to be thrown
        }
    }
}
