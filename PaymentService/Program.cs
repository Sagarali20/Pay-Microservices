using Common.Interface;
using PaymentService.Application.Request.Send;
using PaymentService.Helpers;
using PaymentService.Helpers.Service;
using PaymentService.RegistersExtensions;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//logger configuration
var logger = new LoggerConfiguration()
 .ReadFrom.Configuration(builder.Configuration)
 .MinimumLevel.Debug()
 .Enrich.FromLogContext()
 .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<ISendService, SendService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var configuration = builder.Configuration;
var services = builder.Services;
var serviceSettings = services.StartupBoostrap(configuration);
services.AddSingleton(serviceSettings);
services.AddConsulSettings(serviceSettings);
services.AddHealthChecks();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("UAT"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
CurrentUserInfo.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// UseRouting must come before UseEndpoints
app.UseRouting();

app.UseConsul(serviceSettings);

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(option =>
{
    option.AllowAnyOrigin();
    option.AllowAnyMethod();
    option.AllowAnyHeader();
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});

app.Run();
