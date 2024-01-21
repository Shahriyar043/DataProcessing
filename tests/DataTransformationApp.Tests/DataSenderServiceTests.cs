using DataProcessing.Api.Models;
using DataProcessing.Api.Services.DataSender;
using DataProcessing.Api.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;

namespace DataProcessing.Tests;

public class DataSenderServiceTests
{
    [Fact]
    public async Task SendDataAsync_SuccessfullySendsDataToQueue()
    {
        // Arrange
        var rabbitMQSettingsMock = new Mock<IOptions<RabbitMQSettings>>();
        rabbitMQSettingsMock.Setup(x => x.Value).Returns(new RabbitMQSettings { HostName = "localhost", QueueName = "test_queue", BatchSize = 1 });

        var loggerMock = new Mock<ILogger<DataSenderService>>();

        var connectionFactoryMock = new Mock<IConnectionFactory>();
        var connectionMock = new Mock<IConnection>();
        var channelMock = new Mock<IModel>();

        connectionFactoryMock.Setup(x => x.CreateConnection()).Returns(connectionMock.Object);
        connectionMock.Setup(x => x.CreateModel()).Returns(channelMock.Object);

        var dataSenderService = new DataSenderService(rabbitMQSettingsMock.Object, loggerMock.Object);

        var persons = new List<Person>
        {
            new Person { Id = 1, Name = "John Doe", IsActive = true, DateOfBirth = new DateTime(1992, 1, 1) }
        };

        // Act
        await dataSenderService.SendDataAsync(persons);
    }
}
