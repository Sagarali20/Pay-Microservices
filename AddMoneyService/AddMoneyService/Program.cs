using AddMoneyService.Application.RequestMoney;
using Common;
using Common.Interface;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using AddMoneyService.Service;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IRequestMoneyService, AddMoneyService.Service.RequestMoneyService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseCors(option =>
{
    option.AllowAnyOrigin();
    option.AllowAnyMethod();
    option.AllowAnyHeader();
});
app.Run();
