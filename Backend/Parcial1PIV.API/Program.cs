using MongoDB.Driver;
using Parcial1PIV.API.Services;
using Parcial1PIV.API.Settings;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Controladores
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuración de Scalar y OpenAPI
builder.Services.AddOpenApi();

// Configuración MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

var mongoSettings = builder.Configuration
    .GetSection("MongoDbSettings")
    .Get<MongoDbSettings>();

if (mongoSettings == null ||
    string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
{
    throw new InvalidOperationException(
        "No se configuró la conexión con MongoDB."
    );
}

// Cliente MongoDB
builder.Services.AddSingleton<IMongoClient>(
    _ => new MongoClient(mongoSettings.ConnectionString)
);

// Servicio de tareas
builder.Services.AddSingleton<TareaService>();

var app = builder.Build();

app.UseCors("AngularPolicy");

// Configuración OpenAPI y Scalar
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Prueba de conexión MongoDB
if (app.Environment.IsDevelopment())
{
    app.MapGet("/api/health/mongodb", async (TareaService service) =>
    {
        await service.PingAsync();

        return Results.Ok(new
        {
            mensaje = "Conexión con MongoDB exitosa"
        });
    });
}

// Rutas de controladores
app.MapControllers();

app.Run();