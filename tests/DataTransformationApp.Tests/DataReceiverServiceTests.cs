using DataProcessing.Api.Services.DataReceiver;
using DataProcessing.Api.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DataProcessing.Tests;

public class DataReceiverServiceTests
{
    [Fact]
    public async Task ReceiveDataAsync_SuccessfullyReceivesDataFromQueue()
    {
        // Arrange
        var rabbitMQSettingsMock = new Mock<IOptions<RabbitMQSettings>>();
        rabbitMQSettingsMock.Setup(x => x.Value).Returns(new RabbitMQSettings { HostName = "localhost", QueueName = "test_queue" });

        var loggerMock = new Mock<ILogger<DataReceiverService>>();

        var dataReceiverService = new DataReceiverService(rabbitMQSettingsMock.Object, loggerMock.Object);

        // Act
        var receivedData = await dataReceiverService.ReceiveDataAsync();

        // Assert
        Assert.NotNull(receivedData);
    }
}
