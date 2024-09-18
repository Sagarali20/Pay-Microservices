using Common;
using Common.Interface;
using RemittanceService.Application.Request.Ramittance;
using RemittanceService.RegistersExtensions;
using Serilog;
using System.Data;
using System.Data.SqlClient;
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

// encryption
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var encryptionHelper = new EncryptionHelper(builder.Configuration["Encryption:Key"], builder.Configuration["Encryption:IV"]);
string decryptedConnectionString = encryptionHelper.Decrypt(connectionString);
builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(decryptedConnectionString));
//builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
var configuration = builder.Configuration;
var services = builder.Services;
var serviceSettings = services.StartupBoostrap(configuration);
services.AddSingleton(serviceSettings);
services.AddConsulSettings(serviceSettings);
services.AddHealthChecks();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IRamittanceService, RemittanceService.Helpers.Service.RamittanceService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseConsul(serviceSettings);
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

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
