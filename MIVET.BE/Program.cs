using Microsoft.EntityFrameworkCore;
using MIVET.BE.Aplicacion.Services;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Infraestructura.Repositories;
using MIVET.BE.Repositorio;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Servicios;
using MIVET.BE.Transversales.Interfaces.Repositories;
using MIVET.BE.Transversales.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS solo en desarrollo para permitir cualquier origen
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
}

// Cargar la cadena de conexión desde user-secrets o appsettings 
var connectionString = builder.Configuration.GetConnectionString("Database");

// Registrar DbContext con PooledFactory 
builder.Services.AddDbContext<MIVETDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPersonaClienteBLL, PersonaClienteBLL>();
builder.Services.AddScoped<IPersonaClienteDAL, PersonaClienteDAL>();
builder.Services.AddScoped<IMascotaBLL, MascotaBLL>();
builder.Services.AddScoped<IMascotasDAL, MascotaDAL>();
builder.Services.AddScoped<IMedicoVeterinarioBLL, MedicoVeterinarioBLL>();
builder.Services.AddScoped<IMedicoVeterinarioDAL, MedicoVeterinarioDAL>();
builder.Services.AddScoped<IHistoriaClinicaMascotaBLL, HistoriaClinicaMascotaBLL>();
builder.Services.AddScoped<IHistoriaClinicaMascotaDAL, HistoriaClinicaMascotaDAL>();
builder.Services.AddScoped<IUsuariosBLL, UsuariosBLL>();
builder.Services.AddScoped<IUsuariosDAL, UsuariosDAL>();
builder.Services.AddScoped<IProductosBLL, ProductosBLL>();
builder.Services.AddScoped<IProductosDAL, ProductosDAL>();
builder.Services.AddScoped<IConsultasBLL, ConsultasBLL>();
builder.Services.AddScoped<IConsultasDAL, ConsultasDAL>();
builder.Services.AddScoped<IHorarioVeterinarioRepository, HorarioVeterinarioRepository>();
builder.Services.AddScoped<IHorarioVeterinarioService, HorarioVeterinarioService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IDisponibilidadService, DisponibilidadService>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();

// Agregar servicios a contenedor 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware para habilitar el rebobinado del body
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// Usar CORS - Debe ir antes de UseRouting y UseAuthorization
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAngularApp");
}

// Configurar middleware 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
