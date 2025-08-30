using Microsoft.AspNetCore.Mvc;
using FinanManager.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanManager.Controllers
{
  public class NotificationsController : Controller
  {
    private readonly FinanManagerContext _context;

    public NotificationsController(FinanManagerContext context)
    {
      _context = context;
    }

    // Método para obtener la vista de notificaciones
    public IActionResult Index()
    {
      if (!User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Login", "Auth"); // Redirige al login si no está autenticado
      }

      // Obtener el RoleID del usuario autenticado
      var roleIdString = User.FindFirst("RoleID")?.Value;

      if (string.IsNullOrEmpty(roleIdString))
      {
        return RedirectToAction("Login", "Auth"); // Redirige al login si no hay un RoleID válido
      }

      var roleId = int.Parse(roleIdString);

      // Obtén las notificaciones para el RoleID del usuario logueado
      var notifications = _context.Notifications
          .Include(n => n.Role) // Incluye la relación con Roles
          .Where(n => n.role_ID == roleId) // Filtra por el RoleID del usuario
          .ToList();

      return View(notifications); // Pasa las notificaciones a la vista
    }

    // Método para obtener el número de notificaciones
    public IActionResult GetNotificationCount()
    {
      if (!User.Identity.IsAuthenticated)
      {
        return Json(0); // Devuelve 0 si el usuario no está autenticado
      }

      // Obtener el RoleID del usuario autenticado
      var roleIdString = User.FindFirst("RoleID")?.Value;

      if (string.IsNullOrEmpty(roleIdString))
      {
        return Json(0); // Devuelve 0 si no hay un RoleID válido
      }

      var roleId = int.Parse(roleIdString);

      // Cuenta las notificaciones para el RoleID del usuario logueado
      var count = _context.Notifications
          .Count(n => n.role_ID == roleId); // Filtra por el RoleID del usuario

      return Json(count); // Devuelve el número como JSON
    }

    // Método para obtener las notificaciones en formato JSON
    public IActionResult GetNotifications()
    {
      if (!User.Identity.IsAuthenticated)
      {
        return Json(new { error = "El usuario no está autenticado." });
      }

      // Obtener el RoleID del usuario autenticado
      var roleIdString = User.FindFirst("RoleID")?.Value;

      if (string.IsNullOrEmpty(roleIdString))
      {
        return Json(new { error = "El usuario no tiene un RoleID válido." });
      }

      var roleId = int.Parse(roleIdString);

      // Obtén las notificaciones para el RoleID del usuario logueado
      var notifications = _context.Notifications
          .Include(n => n.Role)
          .Where(n => n.role_ID == roleId)
          .Select(n => new
          {
            n.NotificationsId, // Asegúrate de incluir el ID de la notificación
            n.NotificationType,
            n.NotificationMessage,
            role_name = n.Role.role_name
          })
          .ToList();

      // Log para depuración
      Console.WriteLine($"Notificaciones obtenidas: {notifications.Count}");

      return Json(notifications);
    }

    // Método para eliminar una notificación
    [HttpPost]
    public IActionResult DeleteNotification(int id)
    {
      if (!User.Identity.IsAuthenticated)
      {
        return Json(new { success = false, message = "El usuario no está autenticado." });
      }

      var notification = _context.Notifications.Find(id);
      if (notification == null)
      {
        return Json(new { success = false, message = "Notificación no encontrada." });
      }

      _context.Notifications.Remove(notification);
      _context.SaveChanges();

      return Json(new { success = true, message = "Notificación eliminada correctamente." });
    }
  }
}
