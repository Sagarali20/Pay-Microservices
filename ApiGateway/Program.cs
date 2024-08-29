using ApiGateway.Helpers.Interface;
using AuthenticationService.Helpers;
using AuthenticationService.Utils;
using JwtAuthenticationManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
     .AddJsonFile("ocelot.json")
     .AddEnvironmentVariables();
builder.Services.AddOcelot().AddConsul();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCustomJwtAuthentication();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddAuthorization(options =>
//{
//    //options.AddPolicy("RequireAdministratorRole", policy =>
//    //    policy.RequireClaim("Role", "User"));
//    options.AddPolicy("RequireAdministratorRole", policy =>
//    policy.RequireAuthenticatedUser());
//});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ApiKeyAuthHandler>();
await app.UseOcelot();

app.Run();
