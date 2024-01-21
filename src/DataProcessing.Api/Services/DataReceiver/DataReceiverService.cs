using DataProcessing.Api.Models;
using DataProcessing.Api.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DataProcessing.Api.Services.DataReceiver;

public class DataReceiverService : IDataReceiverService
{
    #region Fields and Constructor

    private readonly IOptions<RabbitMQSettings> rabbitMQSettings;
    private readonly ILogger<DataReceiverService> logger;
    private AutoResetEvent messageReceived = new AutoResetEvent(false);

    public DataReceiverService(IOptions<RabbitMQSettings> rabbitMQSettings, ILogger<DataReceiverService> logger)
    {
        this.rabbitMQSettings = rabbitMQSettings ?? throw new ArgumentNullException(nameof(rabbitMQSettings));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region IDataReceiverService Implementation

    public Task<IEnumerable<Person>> ReceiveDataAsync()
    {
        var receivedPersons = new List<Person>();

        using (var connection = CreateRabbitMQConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: rabbitMQSettings.Value.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => ProcessReceivedMessage(ea, receivedPersons);

            channel.BasicConsume(queue: rabbitMQSettings.Value.QueueName, autoAck: true, consumer: consumer);

            messageReceived.WaitOne();
        }

        logger.LogInformation("Data received successfully from the message queue");

        return Task.FromResult<IEnumerable<Person>>(receivedPersons);
    }

    #endregion

    #region Private Methods

    private IConnection CreateRabbitMQConnection()
    {
        var factory = new ConnectionFactory() { HostName = rabbitMQSettings.Value.HostName };
        return factory.CreateConnection();
    }

    private void ProcessReceivedMessage(BasicDeliverEventArgs ea, List<Person> receivedPersons)
    {
        var body = ea.Body.ToArray();
        var jsonMessage = Encoding.UTF8.GetString(body);
        var persons = JsonSerializer.Deserialize<List<Person>>(jsonMessage);

        if (persons is null || !persons.Any())
        {
            throw new ArgumentNullException(nameof(persons), "The collection of persons cannot be null or empty.");
        }

        receivedPersons.AddRange(persons);
        foreach (var person in persons)
        {
            logger.LogInformation($"Received Person: {person.Id}, {person.Name}, {person.DateOfBirth}, {person.IsActive}");
        }

        messageReceived.Set();
    }

    #endregion
}
