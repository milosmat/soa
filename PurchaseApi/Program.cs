using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using PurchaseApi.Store;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var mongoConn = Environment.GetEnvironmentVariable("MONGO__CONNECTIONSTRING") ?? "mongodb://localhost:27017";
var mongoDb = Environment.GetEnvironmentVariable("MONGO__DATABASE") ?? "soa_purchase";
var jwtSecret = Environment.GetEnvironmentVariable("JWT__SECRET") ?? "devsecret-change-me";
var port = Environment.GetEnvironmentVariable("PORT") ?? "8091";

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
builder.Services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDb));
builder.Services.AddSingleton<AppDb>();

var toursApiUrl = Environment.GetEnvironmentVariable("TOURS_API") ?? "http://localhost:8090";
builder.Services.AddHttpClient("ToursApi", c => { c.BaseAddress = new Uri(toursApiUrl); });

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Purchase API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Unesi JWT token (bez 'Bearer ').",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

builder.Services.AddCors();

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

app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
