using AgroMarketApi.data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 1) Configuración de base de datos
// ======================================================
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================================================
// 2) Configuración de CORS, controladores y Swagger
// ======================================================
builder.Services.AddControllers();

// Política CORS global (nombrada para que se use explícitamente)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================================================
// 3) Construcción de la aplicación
// ======================================================
var app = builder.Build();

// ======================================================
// 4) Middlewares
// ======================================================

// ⚠️ Muy importante: usar la política CORS ANTES de mapear controladores
app.UseCors("AllowAll");

// Solo redirigir HTTPS si estás en producción
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware de autorización (si lo usas más adelante)
app.UseAuthorization();

// ======================================================
// 5) Endpoints
// ======================================================
app.MapControllers();

// Health check rápido
app.MapGet("/", () => Results.Ok(new
{
    ok = true,
    api = "AgroMarketApi",
    time = DateTime.UtcNow
}));

// ======================================================
// 6) Ejecutar aplicación
// ======================================================
app.Run();
