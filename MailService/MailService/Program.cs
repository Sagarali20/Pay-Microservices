using MailKit;
using MailService.Config;
using MailService.Interfaces;
using MailService.Services;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMailServices, MailServices>();
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddHostedService<RabbitMQHostedService>();
builder.Services.AddScoped<IIDbConnection, DbConnection>();
builder.Services.AddScoped<IMailLogService, MailLogService>();
//logger configuration
var logger = new LoggerConfiguration()
 .ReadFrom.Configuration(builder.Configuration)
 .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
