using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.Models;
using Hotel.Services; 
using Hotel.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hotel.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración del servicio CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Permite a tu Angular
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
var issuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// DbContext
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// FluentValidation - Registro de validadores
builder.Services.AddScoped<RegistroRequestValidator>();
builder.Services.AddScoped<LoginRequestValidator>();
builder.Services.AddScoped<ActualizarUsuarioValidator>();
builder.Services.AddScoped<CrearHabitacionValidator>();
builder.Services.AddScoped<ActualizarHabitacionValidator>();
builder.Services.AddScoped<AgregarFotoValidator>();

// Servicios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IHabitacionService, HabitacionService>();
builder.Services.AddScoped<JwtService>(); 
builder.Services.AddScoped<ITemporadaPrecioService, TemporadaPrecioService>();
builder.Services.AddScoped<ITemporadaHabitacionPrecioService, TemporadaHabitacionPrecioService>();
builder.Services.AddScoped<IHuespedService, HuespedService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<ITipoHabitacionService, TipoHabitacionService>();
builder.Services.AddHostedService<Hotel.Services.ReservaBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ---------------------------------------------------------
// CORRECCIÓN IMPORTANTE:
// UseCors debe ir ANTES de UseAuthentication, UseAuthorization y MapControllers
// ---------------------------------------------------------
app.UseCors("AllowAngular"); 

app.UseAuthentication();  
app.UseAuthorization();   

app.MapControllers();

app.Run();