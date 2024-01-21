using DataProcessing.Api.Extensions;
using DataProcessing.Api.Models;
using DataProcessing.Api.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace DataProcessing.Api.Services.DataSender;

public class DataSenderService : IDataSenderService
{
    #region Fields and Constructor

    private readonly IOptions<RabbitMQSettings> rabbitMQSettings;
    private readonly ILogger<DataSenderService> logger;

    public DataSenderService(IOptions<RabbitMQSettings> rabbitMQSettings, ILogger<DataSenderService> logger)
    {
        this.rabbitMQSettings = rabbitMQSettings ?? throw new ArgumentNullException(nameof(rabbitMQSettings));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region IDataSenderService Implementation

    public async Task SendDataAsync(IEnumerable<Person> persons)
    {
        if (persons is null)
        {
            throw new ArgumentNullException(nameof(persons));
        }

        var batches = persons.Select((person, index) => new { person, index })
                             .GroupBy(item => item.index / rabbitMQSettings.Value.BatchSize)
                             .Select(group => group.Select(item => item.person).ToList())
                             .ToList();

        foreach (var batch in batches)
        {
            await SendPersonsData(batch);
            await Task.Delay(TimeSpan.FromMinutes(1));
        }

        logger.LogInformation("Data sent to the message queue successfully");
    }

    #endregion

    #region Private Methods

    private Task SendPersonsData(IEnumerable<Person> persons)
    {
        using (var connection = CreateRabbitMQConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: rabbitMQSettings.Value.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var message = persons.ToJsonString();
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: rabbitMQSettings.Value.QueueName, basicProperties: null, body: body);
        }

        return Task.CompletedTask;
    }

    private IConnection CreateRabbitMQConnection()
    {
        var factory = new ConnectionFactory() { HostName = rabbitMQSettings.Value.HostName };
        return factory.CreateConnection();
    }

    #endregion
}
