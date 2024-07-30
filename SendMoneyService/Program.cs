using SendMoneyService.Helpers;
using SendMoneyService.RegistersExtensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

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
if (app.Environment.IsDevelopment())
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});

app.Run();
