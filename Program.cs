using Microsoft.EntityFrameworkCore;
using TallerMecanicoAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Inyectar ApplicationDbContext configurado con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL")));

// 2. Servicios de Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Crear la base de datos y la tabla automáticamente si no existen (sin usar migraciones)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// 4. Configurar Swagger para pruebas
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();