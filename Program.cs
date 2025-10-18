using AgroMarketApi.data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 1) Configuración de base de datos
// ======================================================
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================================================
// 2) CORS, controladores y Swagger
// ======================================================
builder.Services.AddControllers();

// 🚀 Política de CORS explícita y global
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            // 🔧 Acepta tu origen local de desarrollo
            .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
            // 🔧 Acepta también tu dominio público de frontend (agrega el que uses si lo tienes)
            .SetIsOriginAllowed(origin => true) // ⚠️ permite todos los orígenes (temporal, para Render)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ======================================================
// 3) Middleware global (Render-friendly)
// ======================================================

// ✅ CORS ANTES que todo
app.UseCors("AllowFrontend");

// Render ya sirve HTTPS automáticamente — no lo fuerces
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Si usas auth (JWT o similar)
app.UseAuthorization();

// ======================================================
// 4) Endpoints
// ======================================================
app.MapControllers();

// Health check
app.MapGet("/", () => Results.Ok(new
{
    ok = true,
    api = "AgroMarketApi",
    cors = "enabled",
    env = app.Environment.EnvironmentName,
    time = DateTime.UtcNow
}));

// ======================================================
// 5) Run
// ======================================================
app.Run();
