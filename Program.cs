using System.Linq;
using FinanManager.Controllers;
using FinanManager.Models;
using FinanManager.Services;
using Hangfire; // Hangfire
using Hangfire.Dashboard; // Para usar Cast<T>()
using Hangfire.MemoryStorage; // Hangfire.MemoryStorage
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Establecer cultura global en español de Costa Rica
var defaultCulture = new CultureInfo("es-CR");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

// Configurar la base de datos
builder.Services.AddDbContext<FinanManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinanManagerDBConnection")));

// Registrar servicios adicionales
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IEmailSender, EmailSender>(); // Registrar IEmailSender
builder.Services.AddHttpContextAccessor();

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

// Configurar Hangfire
builder.Services.AddHangfire(config =>
{
  config.UseMemoryStorage(); // Usar MemoryStorage para Hangfire (solo para desarrollo)
});
builder.Services.AddHangfireServer(); // Agregar el servidor de Hangfire

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configurar el dashboard de Hangfire (solo para desarrollo)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
  Authorization = new[] { new HangfireAuthorizationFilter() }.Cast<IDashboardAuthorizationFilter>() // Conversión explícita
});

// Configuración de la ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=LoginBasic}/{id?}");

// Configuración de rutas adicionales
app.MapControllerRoute(
    name: "VerPresupuesto",
    pattern: "VerPresupuesto/{action=Index}/{id?}",
    defaults: new { controller = "VerPresupuesto" });

app.Run();

// Filtro de autorización para el dashboard de Hangfire
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
  public bool Authorize(DashboardContext context)
  {
    return true; // Permitir acceso a todos (solo para desarrollo)
  }
}
