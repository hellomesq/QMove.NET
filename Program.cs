using Microsoft.EntityFrameworkCore;
using MotoMonitoramento.Data;

var builder = WebApplication.CreateBuilder(args);

// Oracle EF Core com pooling
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"))
);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Urls.Add($"http://*:{port}");

app.Run();
