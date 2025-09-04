using MotoMonitoramento.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Oracle EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// Adiciona suporte a CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Permite qualquer origem (web, app, etc)
              .AllowAnyMethod()   // Permite GET, POST, PUT, DELETE...
              .AllowAnyHeader();  // Permite qualquer cabeçalho
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pega a porta que o Render fornece
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Urls.Add($"http://*:{port}");

// Habilita CORS antes de MapControllers
app.UseCors("AllowAll");

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Redireciona a raiz para o Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseAuthorization();
app.MapControllers();

app.Run();
