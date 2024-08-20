using CommonService.Application.Request.CommonGet;
using CommonService.Helpers;
using CommonService.Helpers.Service;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<ICommonGetService, CommonGetService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(p => p.AddPolicy("PSP", policy =>
{
    policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();

}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging() || app.Environment.IsEnvironment("UAT"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
CurrentUserInfo.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseAuthorization();

app.MapControllers();

app.Run();
