using AgroMarketApi.data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) DB (tu cadena está en appsettings.json → "DefaultConnection")
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Controllers + CORS + Swagger
builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3) Middleware
app.UseCors();

// Evita el warning/redirect en local (no fuerces HTTPS en dev)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Swagger para probar
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4) Rutas
app.MapControllers();

// Health check rápido
app.MapGet("/", () => Results.Ok(new { ok = true, api = "AgroMarketApi", time = DateTime.UtcNow }));

app.Run();
