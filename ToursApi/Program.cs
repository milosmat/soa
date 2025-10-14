using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ToursApi.Store;
using Microsoft.OpenApi.Models;

using NATS.Client.Core;
using ToursApi.Messaging;
using ToursApi.Messaging.Nats;
using ToursApi.Saga;

var builder = WebApplication.CreateBuilder(args);

var mongoConn = Environment.GetEnvironmentVariable("MONGO__CONNECTIONSTRING") ?? "mongodb://localhost:27017";
var mongoDb = Environment.GetEnvironmentVariable("MONGO__DATABASE") ?? "soa_tour";
var jwtSecret = Environment.GetEnvironmentVariable("JWT__SECRET") ?? "devsecret-change-me";
var port = Environment.GetEnvironmentVariable("PORT") ?? "8090";

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
builder.Services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDb));
builder.Services.AddSingleton<AppDb>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
  {
      ValidateIssuer = false,
      ValidateAudience = false,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
  });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tours API", Version = "v1" });
    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Unesi JWT token (bez 'Bearer ').",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });
});
builder.Services.AddCors();

builder.Services.AddSingleton(sp =>
{
    var host = Environment.GetEnvironmentVariable("NATS__HOST") ?? "localhost";
    var portN = Environment.GetEnvironmentVariable("NATS__PORT") ?? "4222";
    return new NatsConnection(new NatsOpts { Url = $"nats://{host}:{portN}" });
});
builder.Services.AddSingleton<IPublisher, NatsPublisher>();
builder.Services.AddSingleton<ISubscriber, NatsSubscriber>();

builder.Services.AddSingleton<ReservationCommandHandler>();

var app = builder.Build();

app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

_ = app.Services.GetRequiredService<ReservationCommandHandler>().StartAsync();

app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
