using CourierMax.API.Middleware;
using CourierMax.Data;
using CourierMax.UseCases;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddData(builder.Configuration)
    .AddUseCases();

builder.Services.AddControllers();
// OpenAPI + Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Manejo centralizado de errores
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger UI disponible en /swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
