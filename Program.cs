using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MotoMonitoramento.Data;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 🔹 CONFIGURAÇÕES DE SERVIÇOS
// =======================================================

// Banco de dados Oracle
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options
        .UseOracle(builder.Configuration.GetConnectionString("OracleConnection"))
        .EnableSensitiveDataLogging()
);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

// =======================================================
// 🔹 HEALTH CHECKS
// =======================================================
builder.Services.AddHealthChecks();

// =======================================================
// 🔹 VERSIONAMENTO DE API
// =======================================================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // permite /api/v1/
});

// =======================================================
// 🔹 AUTENTICAÇÃO (JWT)
// =======================================================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChaveSuperSecreta123!"; // define no appsettings.json depois
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "QMoveAPI";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// =======================================================
// 🔹 CONSTRUÇÃO DO APP
// =======================================================
var app = builder.Build();

// Middleware
app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "QMove API v1");
    c.RoutePrefix = "swagger";
});

// Health Check endpoint
app.MapHealthChecks("/health");

// Autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Rota simples para teste
app.MapGet("/", () => "✅ API QMove funcionando!");

// Porta (para deploy)
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Urls.Add($"http://*:{port}");

// Ambiente de desenvolvimento
#if DEBUG
app.UseDeveloperExceptionPage();
#endif

app.Run();
