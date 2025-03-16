using Microsoft.AspNetCore.Mvc;
using FinanManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FinanManager.Controllers
{
  public class PerfilController : Controller
  {
    private readonly string _connectionString;
    private readonly ILogger<PerfilController> _logger;

    public PerfilController(IConfiguration configuration, ILogger<PerfilController> logger)
    {
      _connectionString = configuration.GetConnectionString("FinanManagerDBConnection");
      _logger = logger;
    }

    // GET: Perfil/Editar
    public IActionResult Editar()
    {
      // Obtener el ID del usuario actual
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
      if (userIdClaim == null)
      {
        return RedirectToAction("LoginBasic", "Auth"); // Redirige al login si no está autenticado
      }

      var userId = int.Parse(userIdClaim.Value);

      // Obtener los datos del usuario desde la base de datos
      var user = GetUserById(userId);
      if (user == null)
      {
        return NotFound();
      }

      return View(user);
    }

    // POST: Perfil/EditarNombre
    [HttpPost]
    public IActionResult EditarNombre(int userId, string name)
    {
      try
      {
        UpdateUserName(userId, name);
        TempData["SuccessMessage"] = "Nombre actualizado correctamente.";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error al actualizar el nombre.");
        TempData["ErrorMessage"] = "Ocurrió un error al actualizar el nombre.";
      }

      return RedirectToAction("Editar");
    }

    // POST: Perfil/EditarApellido
    [HttpPost]
    public IActionResult EditarApellido(int userId, string lastName)
    {
      try
      {
        UpdateUserLastName(userId, lastName);
        TempData["SuccessMessage"] = "Apellido actualizado correctamente.";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error al actualizar el apellido.");
        TempData["ErrorMessage"] = "Ocurrió un error al actualizar el apellido.";
      }

      return RedirectToAction("Editar");
    }

    // POST: Perfil/EditarCorreo
    [HttpPost]
    public IActionResult EditarCorreo(int userId, string userEmail)
    {
      try
      {
        UpdateUserEmail(userId, userEmail);
        TempData["SuccessMessage"] = "Correo electrónico actualizado correctamente.";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error al actualizar el correo electrónico.");
        TempData["ErrorMessage"] = "Ocurrió un error al actualizar el correo electrónico.";
      }

      return RedirectToAction("Editar");
    }

    // POST: Perfil/EditarContraseña
    [HttpPost]
    public IActionResult EditarContraseña(int userId, string currentPassword, string newPassword, string confirmPassword)
    {
      try
      {
        // Verificar que la contraseña actual sea correcta
        var user = GetUserById(userId);
        if (!VerifyPassword(currentPassword, user.Password))
        {
          TempData["ErrorMessage"] = "La contraseña actual es incorrecta.";
          return RedirectToAction("Editar");
        }

        // Verificar que la nueva contraseña y la confirmación coincidan
        if (newPassword != confirmPassword)
        {
          TempData["ErrorMessage"] = "La nueva contraseña y la confirmación no coinciden.";
          return RedirectToAction("Editar");
        }

        // Cifrar la nueva contraseña
        string hashedPassword = HashPassword(newPassword);

        // Actualizar la contraseña
        UpdateUserPassword(userId, hashedPassword);
        TempData["SuccessMessage"] = "Contraseña actualizada correctamente.";
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error al actualizar la contraseña.");
        TempData["ErrorMessage"] = "Ocurrió un error al actualizar la contraseña.";
      }

      return RedirectToAction("Editar");
    }

    private User GetUserById(int userId)
    {
      using (var connection = new SqlConnection(_connectionString))
      {
        connection.Open();
        var command = new SqlCommand("SELECT * FROM Users WHERE users_ID = @UserId", connection);
        command.Parameters.AddWithValue("@UserId", userId);

        using (var reader = command.ExecuteReader())
        {
          if (reader.Read())
          {
            return new User
            {
              Users_Id = reader.GetInt32(0),
              Name = reader.GetString(1),
              LastName = reader.GetString(2),
              Password = reader.GetString(3),
              UserEmail = reader.GetString(6)
            };
          }
        }
      }

      return null;
    }

    private void UpdateUserName(int userId, string name)
    {
      using (var connection = new SqlConnection(_connectionString))
      {
        connection.Open();
        var command = new SqlCommand("sp_UpdateUserName", connection)
        {
          CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Users_Id", userId);
        command.Parameters.AddWithValue("@Name", name);

        command.ExecuteNonQuery();
      }
    }

    private void UpdateUserLastName(int userId, string lastName)
    {
      using (var connection = new SqlConnection(_connectionString))
      {
        connection.Open();
        var command = new SqlCommand("sp_UpdateUserLastName", connection)
        {
          CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Users_Id", userId);
        command.Parameters.AddWithValue("@LastName", lastName);

        command.ExecuteNonQuery();
      }
    }

    private void UpdateUserEmail(int userId, string userEmail)
    {
      using (var connection = new SqlConnection(_connectionString))
      {
        connection.Open();
        var command = new SqlCommand("sp_UpdateUserEmail", connection)
        {
          CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Users_Id", userId);
        command.Parameters.AddWithValue("@UserEmail", userEmail);

        command.ExecuteNonQuery();
      }
    }

    private void UpdateUserPassword(int userId, string newPassword)
    {
      using (var connection = new SqlConnection(_connectionString))
      {
        connection.Open();
        var command = new SqlCommand("sp_UpdateUserPassword", connection)
        {
          CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Users_Id", userId);
        command.Parameters.AddWithValue("@NewPassword", newPassword);

        command.ExecuteNonQuery();
      }
    }

    private string HashPassword(string password)
    {
      using (SHA256 sha256Hash = SHA256.Create())
      {
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
          builder.Append(bytes[i].ToString("x2"));  // Convertir a hexadecimal
        }
        return builder.ToString();
      }
    }

    private bool VerifyPassword(string inputPassword, string storedPasswordHash)
    {
      string hashedInputPassword = HashPassword(inputPassword);
      return hashedInputPassword == storedPasswordHash;
    }
  }
}
