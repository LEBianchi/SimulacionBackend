using Microsoft.EntityFrameworkCore;
using SimulacionBackend.Data;
using SimulacionBackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=simulaciones.db"));

builder.Services.AddScoped<ISimulacionService, SimulacionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>(); // Fijate que el nombre coincida con tu DbContext
        context.Database.EnsureCreated(); // Esto fabrica el archivo SQLite y las tablas de cero
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creando la BD: {ex.Message}");
    }
}


app.UseHttpsRedirection();
app.UseCors("PermitirFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();