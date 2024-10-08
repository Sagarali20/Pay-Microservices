using JwtAuthenticationManager;
using AuthenticationService.Helpers.Interface;
using AuthenticationService.Helpers;
using System.Reflection;
using AuthenticationService.Application.Request.Login;
//using AuthenticationService.Helpers.Service;
using AuthenticationService.RegistersExtensions;
using AuthenticationService.Helpers.Service;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//logger configuration
var logger = new LoggerConfiguration()
 .ReadFrom.Configuration(builder.Configuration)
 .MinimumLevel.Debug()
 .Enrich.FromLogContext()
 .CreateLogger();

// redis cache configuration
/*builder.Services.AddStackExchangeRedisCache(
option =>
{
    var connection = builder.Configuration.GetConnectionString("Redis");
    option.Configuration = connection;
});*/

/*var connection = builder.Configuration.GetConnectionString("Redis");*/
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));


builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IPermissionServicecs, PermissionService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var configuration = builder.Configuration;
var services = builder.Services;
var serviceSettings = services.StartupBoostrap(configuration);
services.AddSingleton(serviceSettings);
services.AddConsulSettings(serviceSettings);
services.AddHealthChecks();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCustomJwtAuthentication();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<JwtTokenHandler>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging() || app.Environment.IsEnvironment("UAT"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(option =>
{
    option.AllowAnyOrigin();
    option.AllowAnyMethod();
    option.AllowAnyHeader();
});
app.UseStaticFiles();


CurrentUserInfo.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.UseConsul(serviceSettings);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});


app.Run();
