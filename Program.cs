using AgroMarketApi.data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========== 1) DB ==========
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== 2) CORS + Controllers + Swagger ==========
const string AllowFrontend = "AllowFrontend";

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowFrontend, policy =>
    {
        policy
            // Orígenes permitidos: tu Render + local
            .WithOrigins(
                "https://agromarket-s920.onrender.com",
                "http://localhost:5500",
                "http://127.0.0.1:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
            // .AllowCredentials(); // sólo si usas cookies/autenticación con credenciales
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ========== 3) Binding al puerto de Render ==========
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// ========== 4) Pipeline ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Orden recomendado para CORS con endpoint routing:
app.UseRouting();
app.UseCors(AllowFrontend);

app.UseAuthorization();

// ========== 5) Endpoints ==========
app.MapControllers();

// Respuesta a preflights (OPTIONS) de forma genérica:
app.MapMethods("/{**any}", new[] { "OPTIONS" }, () => Results.Ok())
   .AllowAnonymous();

// Health checks (uno sin DB y otro con DB si quieres)
app.MapGet("/", () => Results.Ok(new
{
    ok = true,
    api = "AgroMarketApi",
    cors = "enabled",
    env = app.Environment.EnvironmentName,
    time = DateTime.UtcNow
}));

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.Run();
