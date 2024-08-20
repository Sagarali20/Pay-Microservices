using System.Text;
using MailService.Interfaces;
using MailService.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace MailService.Services;
public class RabbitMQService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMQSetting _rabbitMQSetting;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMailServices _mailServices;
    private readonly ILogger<RabbitMQService> _logger;
    public RabbitMQService(IConfiguration config, IServiceProvider serviceProvider, ILogger<RabbitMQService> logger)
    {
        _rabbitMQSetting = config.GetSection("RabbitMQ").Get<RabbitMQSetting>().getCredentials(config["Encryption:Key"], config["Encryption:IV"]);
        var factory = new ConnectionFactory { HostName = _rabbitMQSetting.Host, Port = _rabbitMQSetting.Port };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _serviceProvider = serviceProvider;
        _mailServices = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IMailServices>();
        _logger = logger;
    }

    public void ReceiveMessages()
    {
        _channel.QueueDeclare(queue: _rabbitMQSetting.Queue,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
                MailBody? mailBody = JsonConvert.DeserializeObject<MailBody>(message);
                _mailServices.HandleMails(mailBody);

            }
            catch (Exception exception) {
                _logger.LogError(exception.ToString());
            }
        };
        _channel.BasicConsume(queue: _rabbitMQSetting.Queue,
                             autoAck: true,
                             consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
