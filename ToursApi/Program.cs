using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;  
using Microsoft.IdentityModel.Tokens;                 
using Microsoft.OpenApi.Models;                      
using MongoDB.Driver;
using ToursApi.Store;

var builder = WebApplication.CreateBuilder(args);

// ENV
var mongoConn = Environment.GetEnvironmentVariable("MONGO__CONNECTIONSTRING") ?? "mongodb://localhost:27017";
var mongoDb = Environment.GetEnvironmentVariable("MONGO__DATABASE") ?? "soa_tour";
var jwtSecret = Environment.GetEnvironmentVariable("JWT__SECRET") ?? "this_is_a_dev_secret_with_32+_chars!!";
var port = Environment.GetEnvironmentVariable("PORT") ?? "8090";

// Mongo & DI
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
builder.Services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDb));
builder.Services.AddSingleton<AppDb>();

// Auth
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

// Web
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tour API", Version = "v1" });
    var jwt = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Unesi JWT token (bez 'Bearer ').",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", jwt);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwt, Array.Empty<string>() } });
});
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
