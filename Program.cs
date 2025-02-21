using Microsoft.EntityFrameworkCore;
using FinanManager.Models;
using FinanManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using FinanManager.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Configurar la base de datos
builder.Services.AddDbContext<FinanManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinanManagerDBConnection")));

// Agregar servicio para la configuración del correo
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Registrar controlador AdministradorController con la inyección de la cadena de conexión
builder.Services.AddTransient<AdministradorController>();
builder.Services.AddTransient(x => x.GetRequiredService<IConfiguration>()["ConnectionStrings:FinanManagerDBConnection"]);

// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();

// Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.LoginPath = "/Auth/LoginBasic"; // Ruta al login
      options.ExpireTimeSpan = TimeSpan.FromMinutes(2); // Sesión expira en 2 minutos
      options.SlidingExpiration = true; // Renovar el tiempo de expiración con cada actividad
    });

// Configurar autorización global
builder.Services.AddAuthorization(options =>
{
  options.FallbackPolicy = new AuthorizationPolicyBuilder()
      .RequireAuthenticatedUser() // Requiere autenticación para todas las páginas
      .Build();
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
  // Si no está en desarrollo, usa HSTS (HTTP Strict Transport Security) y HTTPS
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
  app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

// Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Configuración de la ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=LoginBasic}/{id?}");

app.Run();
