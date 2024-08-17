using MailService.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace MailService.Services;

public class RabbitMQHostedService : BackgroundService
{
    private readonly RabbitMQService _rabbitMQService;

    public RabbitMQHostedService(RabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService ?? throw new ArgumentNullException(nameof(rabbitMQService));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.ReceiveMessages();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _rabbitMQService.Dispose();
        base.Dispose();
    }
}
