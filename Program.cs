using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;
using Swashbuckle.AspNetCore.Annotations; // ✨ Importante

var builder = WebApplication.CreateBuilder(args);

// Adiciona DbContext com pooling
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options
        .UseOracle(builder.Configuration.GetConnectionString("OracleConnection"))
        .EnableSensitiveDataLogging() // útil para debug
);

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configura Swagger com Annotations
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations(); // permite anotações
});

var app = builder.Build();

// Middleware
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "QMove API v1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/", () => "API QMove funcionando");

app.UseAuthorization();
app.MapControllers();

// Configura porta (para deploy)
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Urls.Add($"http://*:{port}");

// Ativa Hot Reload em dev
#if DEBUG
app.UseDeveloperExceptionPage();
#endif

app.Run();
