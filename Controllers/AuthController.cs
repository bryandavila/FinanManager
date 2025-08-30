using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinanManager.Services;
using System.Data.SqlTypes;
using FinanManager.Models;


public class AuthController : Controller
{
  private readonly IConfiguration _configuration;
  private readonly IEmailSender _emailSender;

  public AuthController(IConfiguration configuration, IEmailSender emailSender)
  {
    _configuration = configuration;
    _emailSender = emailSender;
  }

  // Página de inicio de sesión (acceso público)
  [AllowAnonymous]
  [HttpGet]
  public IActionResult LoginBasic(string returnUrl = null)
  {
    if (returnUrl != null && returnUrl.Contains("expired=true"))
    {
      TempData["SessionExpired"] = "Tu sesión ha expirado debido a inactividad. Por favor, inicia sesión nuevamente.";
    }

    return View();
  }

  //Iniciar sesion
  [AllowAnonymous]
  [HttpPost]
  public async Task<IActionResult> LoginBasic(string email, string password)
  {
    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
    {
      ViewBag.Error = "Por favor ingrese su correo y contraseña.";
      return View();
    }

    try
    {
      using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("FinanManagerDBConnection")))
      {
        conn.Open();

        // Hash de la contraseña usando SHA256
        using (SHA256 sha256Hash = SHA256.Create())
        {
          byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
          StringBuilder builder = new StringBuilder();
          for (int i = 0; i < bytes.Length; i++)
          {
            builder.Append(bytes[i].ToString("x2"));  // Convertir a hexadecimal
          }
          password = builder.ToString();  // Esto es lo que se pasa a la base de datos
        }

        // Verificar las credenciales en la base de datos
        SqlCommand cmd = new SqlCommand("sp_VerificarCredenciales", conn)
        {
          CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Password", password);

        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
          reader.Read();
          int resultCode = reader.GetInt32(0); // Columna 0: ResultCode
          string resultMessage = reader.GetString(1); // Columna 1: ResultMessage
          string userEmail = reader.IsDBNull(2) ? null : reader.GetString(2); // Columna 2: UserEmail
          int role_ID = reader.IsDBNull(3) ? 0 : reader.GetInt32(3); // Columna 3: RoleID
          int userId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4); // Columna 4: UserID

          // Validar si el role_ID es 22 (Acceso denegado)
          if (role_ID == 22)
          {
            ViewBag.Error = "Acceso denegado. Contacte al administrador.";
            return View();
          }

          if (resultCode == 1)
          {
            // Autenticación exitosa
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, email), // Correo electrónico
                        new Claim(ClaimTypes.Name, email), // Nombre (usamos el correo como nombre)
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // ID del usuario
                        new Claim("RoleID", role_ID.ToString()) // Role ID
                    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
              ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(2), // Sesión expira en 2 minutos
              IsPersistent = false // No mantener la sesión abierta después de cerrar el navegador
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirigir según el role_ID
            if (role_ID == 1)
            {
              return RedirectToAction("Index", "Administrador"); // Redirección específica para administradores
            }
            else if (role_ID >= 3 && role_ID <= 21)
            {
              return RedirectToAction("Index", "GestionPresupuesto"); // Redirección para roles entre 3 y 21
            }
            else
            {
              return RedirectToAction("Index", "Dashboards"); // Redirección estándar
            }
          }
          else if (resultCode == -2)
          {
            ViewBag.Error = "Cuenta bloqueada. Contacte al administrador.";
          }
          else if (resultCode == -3)
          {
            ViewBag.Error = "Cuenta bloqueada debido a múltiples intentos fallidos.";

            // Enviar correo electrónico al usuario
            if (!string.IsNullOrEmpty(userEmail))
            {
              string subject = "Cuenta bloqueada";
              string body = "<p>Su cuenta ha sido bloqueada debido a múltiples intentos fallidos de inicio de sesión.</p>";
              body += "<p>Por favor, contacte al administrador para desbloquear su cuenta.</p>";

              _emailSender.SendEmail(userEmail, subject, body);
            }
          }
          else if (resultCode == -4)
          {
            ViewBag.Error = "Credenciales incorrectas. Intente nuevamente.";
          }
        }
        else
        {
          ViewBag.Error = "Credenciales incorrectas. Intente nuevamente.";
        }
      }
    }
    catch (SqlNullValueException ex)
    {
      ViewBag.Error = "Error en el servidor: Uno o más campos requeridos están vacíos en la base de datos.";
    }
    catch (Exception ex)
    {
      ViewBag.Error = "Error en el servidor: " + ex.Message;
    }
    return View();
  }

  [AllowAnonymous]
  [HttpGet]
  public IActionResult SessionExpired()
  {
    return View();
  }

  // Cerrar sesión
  [HttpPost]
  public async Task<IActionResult> Logout()
  {
    // Cerrar la sesión del usuario
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // Mostrar notificación de cierre de sesión
    TempData["SuccessMessage"] = "Has cerrado sesión correctamente.";

    // Redirigir al login
    return RedirectToAction("LoginBasic", "Auth");
  }

  // Página de restablecimiento de contraseña (acceso público)
  [AllowAnonymous]
  [HttpGet]
  public IActionResult ForgotPasswordBasic(string email)
  {
    if (string.IsNullOrEmpty(email))
    {
      ViewBag.Error = "Por favor ingrese un correo válido.";
      return View();
    }

    try
    {
      using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("FinanManagerDBConnection")))
      {
        conn.Open();

        // Verificar si el correo existe en la base de datos
        SqlCommand checkEmailCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE userEmail = @Email", conn);
        checkEmailCmd.Parameters.AddWithValue("@Email", email);
        int emailCount = (int)checkEmailCmd.ExecuteScalar();

        if (emailCount == 0)
        {
          ViewBag.Error = "El correo ingresado no está registrado.";
          return View();
        }

        // Generar un token único
        string token = Guid.NewGuid().ToString();

        // Registrar el restablecimiento en la base de datos
        SqlCommand cmd = new SqlCommand("sp_RegistrarRestablecimiento", conn)
        {
          CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Token", token);
        cmd.Parameters.AddWithValue("@FechaExpiracion", DateTime.Now.AddHours(1));  // Expira en 1 hora

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
          // Construir el enlace de restablecimiento
          string resetLink = Url.Action("RestablecerContraseña", "Auth", new { token = token, isAdmin = false }, Request.Scheme);

          // Crear el cuerpo del correo
          string subject = "Instrucciones para restablecer tu contraseña";
          string body = "<p>Recibimos una solicitud para restablecer tu contraseña. Haz clic en el siguiente enlace para cambiarla:</p>";
          body += $"<p><a href='{resetLink}'>Restablecer contraseña</a></p>";
          body += "<p>Si no solicitaste este cambio, ignora este correo.</p>";

          // Enviar el correo
          _emailSender.SendEmail(email, subject, body);

          ViewBag.Message = "Correo enviado con instrucciones para restablecer la contraseña.";
        }
        else
        {
          ViewBag.Error = "Hubo un problema al generar el token. Intenta nuevamente.";
        }
      }
    }
    catch (Exception ex)
    {
      ViewBag.Error = "Error en el servidor: " + ex.Message;
    }
    return View();
  }


  [AllowAnonymous]
  [HttpGet]
  public IActionResult RestablecerContraseña(string token, bool isAdmin = false)
  {
    if (string.IsNullOrEmpty(token))
    {
      ViewBag.Error = "Token inválido.";
      return View();
    }

    try
    {
      using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("FinanManagerDBConnection")))
      {
        conn.Open();

        // Validar el token
        string storedProcedure = isAdmin ? "sp_ValidarTokenAdmin" : "sp_ValidarToken";
        SqlCommand cmd = new SqlCommand(storedProcedure, conn)
        {
          CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Token", token);

        var email = cmd.ExecuteScalar() as string;
        if (string.IsNullOrEmpty(email))
        {
          ViewBag.Error = "Token inválido o expirado.";
          return View();
        }

        ViewBag.Token = token; // Pasar el token a la vista
        ViewBag.IsAdmin = isAdmin; // Pasar el tipo de usuario a la vista
        return View();
      }
    }
    catch (Exception ex)
    {
      ViewBag.Error = "Error en el servidor: " + ex.Message;
      return View();
    }
  }



  // Página de confirmación de restablecimiento de contraseña (acceso público)
  [AllowAnonymous]
  [HttpGet]
  public IActionResult PasswordResetConfirmation()
  {
    return View();
  }


  [AllowAnonymous]
  [HttpPost]
  public IActionResult RestablecerContraseña(string token, string newPassword, string confirmPassword, bool isAdmin = false)
  {
    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
    {
      ViewBag.Error = "Todos los campos son obligatorios.";
      ViewBag.Token = token; // Conservar el token
      ViewBag.NewPassword = newPassword; // Conservar el valor de newPassword
      ViewBag.ConfirmPassword = confirmPassword; // Conservar el valor de confirmPassword
      ViewBag.IsAdmin = isAdmin; // Conservar el tipo de usuario
      return View();
    }

    if (newPassword != confirmPassword)
    {
      ViewBag.Error = "Las contraseñas no coinciden.";
      ViewBag.Token = token; // Conservar el token
      ViewBag.NewPassword = newPassword; // Conservar el valor de newPassword
      ViewBag.ConfirmPassword = confirmPassword; // Conservar el valor de confirmPassword
      ViewBag.IsAdmin = isAdmin; // Conservar el tipo de usuario
      return View();
    }

    try
    {
      using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("FinanManagerDBConnection")))
      {
        conn.Open();

        // Validar el token
        string storedProcedure = isAdmin ? "sp_ValidarTokenAdmin" : "sp_ValidarToken"; //esta dando false
        SqlCommand cmd = new SqlCommand(storedProcedure, conn)
        {
          CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Token", token);

        var email = cmd.ExecuteScalar() as string;
        if (string.IsNullOrEmpty(email)) //esta null
        {
          ViewBag.Error = "Token inválido o expirado.";
          ViewBag.Token = token; // Conservar el token
          ViewBag.IsAdmin = isAdmin; // Conservar el tipo de usuario
          return View();
        }

        // Hash de la nueva contraseña
        string hashedPassword;
        using (SHA256 sha256Hash = SHA256.Create())
        {
          byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
          StringBuilder builder = new StringBuilder();
          for (int i = 0; i < bytes.Length; i++)
          {
            builder.Append(bytes[i].ToString("x2"));  // Convertir a hexadecimal
          }
          hashedPassword = builder.ToString();
        }

        // Actualizar la contraseña en la base de datos
        SqlCommand updateCmd = new SqlCommand("sp_ActualizarContraseñaCorreoElectronico", conn)
        {
          CommandType = CommandType.StoredProcedure
        };
        updateCmd.Parameters.AddWithValue("@Email", email);
        updateCmd.Parameters.AddWithValue("@NewPassword", hashedPassword);

        int rowsAffected = updateCmd.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
          // Enviar correo electrónico de notificación
          string subject = "Contraseña restablecida exitosamente";
          string body = "<p>Tu contraseña ha sido restablecida exitosamente.</p>";
          body += "<p>Si no realizaste este cambio, por favor contacta al administrador.</p>";

          _emailSender.SendEmail(email, subject, body);

          // Redirigir a la vista de confirmación
          return RedirectToAction("PasswordResetConfirmation", "Auth");
        }
        else
        {
          ViewBag.Error = "No se pudo actualizar la contraseña. Verifique el correo electrónico.";
        }
      }
    }
    catch (Exception ex)
    {
      ViewBag.Error = "Error en el servidor: " + ex.Message;
    }

    return View();
  }

}
