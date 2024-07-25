using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using TrendDataIngestion;
using TrendEventData;

[TestClass]
public class EventHubTriggerFunctionTests
{
    private Mock<ITrendDataRepository> _mockTrendDataRepository;
    private Mock<ILogger<EventHubTriggerFunction>> _mockLogger;
    private EventHubTriggerFunction _function;

    [TestInitialize]
    public void Initialize()
    {
        _mockTrendDataRepository = new Mock<ITrendDataRepository>();
        _mockLogger = new Mock<ILogger<EventHubTriggerFunction>>();

        var mockLoggerProvider = new Mock<ILoggerProvider>();
        mockLoggerProvider.Setup(p => p.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

        var mockFunctionContext = new Mock<FunctionContext>();

        // Setup FunctionContext to return _mockLogger.Object for GetLogger<EventHubTriggerFunction>
        mockFunctionContext
            .Setup(c => c.GetLogger<EventHubTriggerFunction>())
            .Returns(_mockLogger.Object);

        _function = new EventHubTriggerFunction(
            _mockTrendDataRepository.Object,
            _mockLogger.Object);
    }

    [TestMethod]
    public async Task RunAsync_SuccessfulEventProcessing()
    {
        // Arrange
        var jsonData = @"
            {
                ""d"": ""5865aab4-3a1f-4ee6-85d3-ac26da75b742"",
                ""trend"": [
                    {
                        ""Tag"": ""Temp"",
                        ""V"": 34,
                        ""T"": 1721034088
                    },
                    {
                        ""Tag"": ""Humidity"",
                        ""V"": 23,
                        ""T"": 1721034088
                    }
                ]
            }";

        var mockContext = new Mock<FunctionContext>();

        // Act
        await _function.RunAsync(new string[] { jsonData }, mockContext.Object);

        // Assert
        // Verify that StoreDataAsync was called twice with expected eventDataEntity objects
        _mockTrendDataRepository.Verify(repo => repo.TrendDataInsertionAsync(It.IsAny<TrendDataTableEntity>()), Times.Exactly(2));
        // Verify logging of success messages
        _mockLogger.Verify(logger => logger.LogInformation(It.IsAny<string>()), Times.Exactly(2));
    }

    [TestMethod]
    public async Task RunAsync_ExceptionHandling()
    {
        // Arrange
        var eventDataJson = "{ invalid json }";  // Simulating invalid JSON data
        var mockContext = new Mock<FunctionContext>();

        // Act
        await _function.RunAsync(new string[] { eventDataJson }, mockContext.Object);

        // Assert
        // Verify that logger.LogError was called once (simulating error logging)
        _mockLogger.Verify(logger => logger.LogError(It.IsAny<string>()), Times.Once);
    }
}
