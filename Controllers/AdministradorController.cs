using Microsoft.AspNetCore.Mvc;
using FinanManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinanManager.Services;
using System.Formats.Asn1;
using CsvHelper;
using Microsoft.SqlServer.Server;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanManager.Controllers
{
  [AuthorizeRole(1)]
  public class AdministradorController : Controller
  {
    private readonly string _connectionString;
    private readonly IEmailSender _emailSender;

    public AdministradorController(string connectionString, IEmailSender emailSender)
    {
      _connectionString = connectionString;
      _emailSender = emailSender;
    }


    public async Task<IActionResult> Index()
    {
      try
      {
        using (var connection = new SqlConnection(_connectionString))
        {
          // Obtener la lista de usuarios con roles
          var users = await connection.QueryAsync<User, Role, User>(
              "sp_GetAllUsers",
              (user, role) =>
              {
                user.Role = role; // Asignar el rol al usuario
                return user;
              },
              splitOn: "role_ID", // Indicar dónde comienza el mapeo del rol
              commandType: CommandType.StoredProcedure
          );

          // Extraer roles únicos de la lista de usuarios
          var uniqueRoles = users
              .Select(u => u.Role)
              .Where(r => r != null)
              .GroupBy(r => r.role_ID) // Usar role_ID en minúsculas
              .Select(g => g.First())
              .ToList();

          // Pasar los roles únicos a la vista
          ViewBag.Roles = uniqueRoles.Select(r => new SelectListItem
          {
            Value = r.role_name,
            Text = r.role_name
          }).ToList();

          return View(users);
        }
      }
      catch (Exception ex)
      {
        ViewBag.Error = "Error al obtener los usuarios: " + ex.Message;
        return View();
      }
    }

    // GET: User/Create
    public async Task<IActionResult> Create()
    {
      try
      {
        using (var connection = new SqlConnection(_connectionString))
        {
          var roles = await connection.QueryAsync<Role>("SELECT role_ID, role_name FROM Roles");
          ViewBag.Roles = roles.Select(r => new SelectListItem { Value = r.role_ID.ToString(), Text = r.role_name }).ToList();
        }
        return View();
      }
      catch (Exception ex)
      {
        ViewBag.Error = "Error al cargar los roles: " + ex.Message;
        return View();
      }
    }

    // POST: User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int id, [Bind("Name,LastName,role_ID,UserEmail")] User user)
    {
      if (id != user.Users_Id)
        return NotFound();

      ModelState.Remove("Role");
      ModelState.Remove("Password");

      if (ModelState.IsValid)
      {
        try
        {
          using (var connection = new SqlConnection(_connectionString))
          {
            var parameters = new { user.Name, user.LastName, role_ID = user.role_ID, user.UserEmail };
            await connection.ExecuteAsync("sp_CreateUser", parameters, commandType: CommandType.StoredProcedure);
          }
          return RedirectToAction(nameof(MensajeConfirmacion));
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Error creando el usuario: " + ex.Message);
          ViewBag.Error = ex.Message;
        }
      }
      else
      {
        ModelState.AddModelError("", "ModelState no es válido");
      }

      // Recargar la lista de roles en caso de error
      try
      {
        using (var connection = new SqlConnection(_connectionString))
        {
          var roles = await connection.QueryAsync<Role>("SELECT role_ID, role_name FROM Roles");
          ViewBag.Roles = roles.Select(r => new SelectListItem { Value = r.role_ID.ToString(), Text = r.role_name }).ToList();
        }
      }
      catch (Exception ex)
      {
        ViewBag.Error = "Error al cargar los roles: " + ex.Message;
      }

      return View(user);
    }

    //Carga CSV
    [HttpPost]
    public async Task<IActionResult> CargarCSV(IFormFile file)
    {
      // Validar si el archivo es nulo o está vacío
      if (file == null || file.Length == 0)
      {
        TempData["Error"] = "Por favor, seleccione un archivo CSV válido.";
        return RedirectToAction("Index");
      }

      // Validar la extensión del archivo
      if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
      {
        TempData["Error"] = "El archivo debe tener la extensión .csv.";
        return RedirectToAction("Index");
      }

      try
      {
        using (var reader = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
        {
          csv.Context.RegisterClassMap<UserCsvMap>();
          var usuariosDto = csv.GetRecords<UserCsvDto>().ToList();

          // Validar si el archivo CSV está vacío
          if (!usuariosDto.Any())
          {
            TempData["Mensaje"] = "El archivo CSV está vacío o tiene un formato incorrecto.";
            TempData["EsError"] = true;
            return RedirectToAction("Index");
          }

          // Validar datos del CSV
          foreach (var usuarioDto in usuariosDto)
          {
            if (string.IsNullOrEmpty(usuarioDto.Name) || string.IsNullOrEmpty(usuarioDto.LastName) || string.IsNullOrEmpty(usuarioDto.UserEmail))
            {
              TempData["Error"] = "El archivo CSV contiene datos incompletos.";
              return RedirectToAction("Index");
            }
          }

          // Insertar datos en la base de datos
          using (var connection = new SqlConnection(_connectionString))
          {
            foreach (var usuarioDto in usuariosDto)
            {
              var parameters = new
              {
                usuarioDto.Name,
                usuarioDto.LastName,
                usuarioDto.Role_ID,
                usuarioDto.UserEmail
              };

              await connection.ExecuteAsync("sp_CreateUser", parameters, commandType: CommandType.StoredProcedure);
            }
          }

          TempData["Mensaje"] = "Datos ingresados correctamente.";
          TempData["EsError"] = false;

          // Redirigir a MensajeConfirmacion después de cargar los datos correctamente
          return RedirectToAction("MensajeConfirmacion");
        }
      }
      catch (CsvHelperException csvEx)
      {
        TempData["Error"] = $"Error en el formato del archivo CSV en la línea {csvEx.Context.Parser.Row}: {csvEx.Message}";
      }
      catch (Exception ex)
      {
        TempData["Error"] = $"Ocurrió un error al procesar el archivo: {ex.Message}";
      }

      return RedirectToAction("Index"); // Mantener la redirección a Index en caso de error
    }




    // GET: User/Edit
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
        return NotFound();

      try
      {
        using (var connection = new SqlConnection(_connectionString))
        {
          var user = await connection.QuerySingleOrDefaultAsync<User>("sp_GetUserById", new { Users_Id = id }, commandType: CommandType.StoredProcedure);

          if (user == null)
            return NotFound();

          var roles = await connection.QueryAsync<Role>("SELECT role_ID, role_name FROM Roles");
          ViewBag.Roles = roles.Select(r => new SelectListItem { Value = r.role_ID.ToString(), Text = r.role_name }).ToList();

          return View(user);
        }
      }
      catch (Exception ex)
      {
        ViewBag.Error = "Error al obtener los datos del usuario para editar: " + ex.Message;
        return View();
      }
    }

    // POST: User/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Users_Id,Name,LastName,role_ID,UserStatus,UserEmail")] User user)
    {
      if (id != user.Users_Id)
        return NotFound();

      ModelState.Remove("Role");
      ModelState.Remove("Password");

      if (ModelState.IsValid)
      {
        try
        {
          using (var connection = new SqlConnection(_connectionString))
          {
            // Obtener el usuario actual antes de la actualización
            var usuarioActual = await connection.QuerySingleOrDefaultAsync<User>(
                "sp_GetUserById",
                new { Users_Id = user.Users_Id },
                commandType: CommandType.StoredProcedure
            );

            // Verificar si el rol ha cambiado
            if (usuarioActual.role_ID != user.role_ID)
            {
              // Registrar el cambio de rol
              await CambiarRol(user.Users_Id, user.role_ID);
            }

            // Actualizar el usuario
            var parameters = new { user.Users_Id, user.Name, user.LastName, role_ID = user.role_ID, user.UserStatus, user.UserEmail };
            await connection.ExecuteAsync("sp_UpdateUser", parameters, commandType: CommandType.StoredProcedure);
          }

          return RedirectToAction(nameof(MensajeConfirmacion));
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Error al actualizar el usuario: " + ex.Message);
          ViewBag.Error = ex.Message;
        }
      }

      // Cargar los roles en caso de error
      try
      {
        using (var connection = new SqlConnection(_connectionString))
        {
          var roles = await connection.QueryAsync<Role>("SELECT role_ID, role_name FROM Roles");
          ViewBag.Roles = roles.Select(r => new SelectListItem { Value = r.role_ID.ToString(), Text = r.role_name }).ToList();
        }
      }
      catch (Exception ex)
      {
        ViewBag.Error = "Error al cargar los roles: " + ex.Message;
      }

      return View(user);
    }

    // POST: User/Delete
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        using (var connection = new SqlConnection(_connectionString))
        {
          var user = await connection.QuerySingleOrDefaultAsync<User>("sp_GetUserById", new { Users_Id = id }, commandType: CommandType.StoredProcedure);

          if (user == null)
            return Json(new { success = false, message = "Usuario no encontrado." });

          await connection.ExecuteAsync("sp_DeleteUser", new { Users_Id = id }, commandType: CommandType.StoredProcedure);

          return Json(new { success = true, message = "Usuario eliminado con éxito." });
        }
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Error al eliminar el usuario: " + ex.Message });
      }
    }

    // POST: User/ActivateOrDeactivateUser
    [HttpPost]
    public async Task<IActionResult> ActivateOrDeactivateUser(int id, int bit)
    {
      try
      {
        using (var connection = new SqlConnection(_connectionString))
        {
          int newStatus = (bit == 1) ? 0 : 1;
          await connection.ExecuteAsync("sp_UpdateUserStatus", new { Users_Id = id, UserStatus = newStatus }, commandType: CommandType.StoredProcedure);

          return Json(new { success = true, message = (newStatus == 1 ? "Usuario activado" : "Usuario desactivado") });
        }
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "Error al activar/desactivar el usuario: " + ex.Message });
      }
    }

    // GET: Administrador/Restablecer/{id}
    [HttpGet]
    public async Task<IActionResult> ResetPassword(int? id)
    {
      if (id == null)
      {
        return NotFound(); // Si no se proporciona un ID, devuelve un error 404
      }

      using (var connection = new SqlConnection(_connectionString))
      {
        // Obtener el usuario desde la base de datos
        var user = await connection.QuerySingleOrDefaultAsync<User>(
            "sp_GetUserById",
            new { Users_Id = id },
            commandType: CommandType.StoredProcedure
        );

        if (user == null)
        {
          return NotFound(); // Si el usuario no existe, devuelve un error 404
        }

        // Mostrar la vista ResetPassword con los datos del usuario
        return View("ResetPassword", user);
      }
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(int Users_Id, string action, string NewPassword, string confimaPassword)
    {
      if (Users_Id == 0)
      {
        ModelState.AddModelError("", "ID de usuario no válido.");
        return View("Error"); // Redirigir a una vista de error
      }

      // Obtener el nombre del administrador responsable
      var responsable = User.Identity.Name; // Obtiene el nombre del usuario autenticado

      using (var connection = new SqlConnection(_connectionString))
      {
        try
        {
          // Obtener el usuario desde la base de datos
          var user = await connection.QuerySingleOrDefaultAsync<User>(
              "sp_GetUserById",
              new { Users_Id },
              commandType: CommandType.StoredProcedure
          );

          if (user == null)
          {
            ModelState.AddModelError("", "Usuario no encontrado.");
            return View("Error"); // Redirigir a una vista de error
          }

          if (action == "manual")
          {
            // Validar que las contraseñas coincidan
            if (NewPassword != confimaPassword)
            {
              ModelState.AddModelError("", "Las contraseñas no coinciden.");
              return View("ResetPassword", user);
            }

            // Hash de la nueva contraseña
            string hashedPassword;
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
              var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(NewPassword));
              hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }

            // Actualizar la contraseña en la base de datos
            await connection.ExecuteAsync(
                "sp_ActualizarContraseñaCorreoElectronico",
                new { Email = user.UserEmail, NewPassword = hashedPassword, /*Responsable = responsable*/ },
                commandType: CommandType.StoredProcedure
            );

            // Registrar el restablecimiento en el historial
            await connection.ExecuteAsync(
                "sp_RegistrarRestablecimientoAdmin",
                new { Email = user.UserEmail, Token = "MANUAL", ExpirationDate = DateTime.Now.AddHours(1), Responsable = responsable },
                commandType: CommandType.StoredProcedure
            );

            // Enviar correo de notificación
            string subject = "Contraseña restablecida";
            string body = $"<p>Tu contraseña ha sido restablecida. Si no realizaste esta acción, por favor contacta al administrador.</p>";
            _emailSender.SendEmail(user.UserEmail, subject, body);

            TempData["Message"] = "Contraseña restablecida correctamente.";
          }
          else if (action == "email")
          {
            // Generar un token único
            string token = Guid.NewGuid().ToString();

            // Registrar el restablecimiento en la base de datos
            await connection.ExecuteAsync(
                "sp_RegistrarRestablecimientoAdmin",
                new { Email = user.UserEmail, Token = token, ExpirationDate = DateTime.Now.AddHours(1), Responsable = responsable },
                commandType: CommandType.StoredProcedure
            );

            // Construir el enlace de restablecimiento
            string resetLink = Url.Action("RestablecerContraseña", "Auth", new { token = token, isAdmin = true }, Request.Scheme);

            // Enviar correo con enlace de restablecimiento
            string subject = "Instrucciones para restablecer tu contraseña";
            string body = $"<p>Recibimos una solicitud para restablecer tu contraseña. Haz clic en el siguiente enlace para cambiarla:</p>";
            body += $"<p><a href='{resetLink}'>Restablecer contraseña</a></p>";
            _emailSender.SendEmail(user.UserEmail, subject, body);

            TempData["Message"] = "Se ha enviado un correo con instrucciones para restablecer la contraseña.";
          }
          else
          {
            ModelState.AddModelError("", "Acción no válida.");
            return View("Error"); // Redirigir a una vista de error
          }

          return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex)
        {
          ModelState.AddModelError("", "Error en la base de datos: " + ex.Message);
          return View("Error"); // Redirigir a una vista de error
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Error en el servidor: " + ex.Message);
          return View("Error"); // Redirigir a una vista de error
        }
      }
    }

    // GET: Administrador/PasswordHistory
    public async Task<IActionResult> PasswordHistory()
    {
      try
      {
        using (var connection = new SqlConnection(_connectionString))
        {
          // Obtener el historial de restablecimientos de contraseña
          var historialRestablecimientos = await connection.QueryAsync<SolicitudesRestablecimientoAdmin>(
              "sp_GetPasswordResetHistory",
              commandType: CommandType.StoredProcedure
          );

          // Obtener el historial de cambios de roles
          var historialCambiosRoles = await connection.QueryAsync<CambioDeRol>(
              "sp_GetRoleChangeHistory",
              commandType: CommandType.StoredProcedure
          );

          // Pasar ambos historiales a la vista
          var model = new AuditoriasViewModel
          {
            Restablecimientos = historialRestablecimientos.ToList(),
            CambiosDeRoles = historialCambiosRoles.ToList()
          };

          return View(model);
        }
      }
      catch (Exception ex)
      {
        ViewBag.Error = "Error al obtener el historial de auditorías: " + ex.Message;
        return View(new AuditoriasViewModel());
      }
    }

    [HttpGet]
    public IActionResult MensajeConfirmacion()
    {
      return View();
    }


    [HttpPost]
    private async Task CambiarRol(int userId, int nuevoRolId)
    {
      try
      {
        // Obtener el nombre del administrador responsable
        var responsable = User.Identity.Name; // Obtiene el nombre del usuario autenticado

        using (var connection = new SqlConnection(_connectionString))
        {
          // Obtener el rol anterior del usuario
          var rolAnteriorId = await connection.QuerySingleOrDefaultAsync<int>(
              "SELECT role_ID FROM Users WHERE users_ID = @userId",
              new { userId }
          );

          // Registrar el cambio de rol en la tabla UserRoleChanges
          await connection.ExecuteAsync(
              "INSERT INTO UserRoleChanges (users_ID, RolAnterior_ID, NuevoRol_ID, FechaCambio, Responsable) " +
              "VALUES (@userId, @rolAnteriorId, @nuevoRolId, @fechaCambio, @responsable)",
              new
              {
                userId,
                rolAnteriorId,
                nuevoRolId,
                fechaCambio = DateTime.Now,
                responsable
              }
          );
        }
      }
      catch (Exception ex)
      {
        // Manejar el error (opcional)
        throw new Exception("Error al registrar el cambio de rol: " + ex.Message);
      }
    }
  }
}
