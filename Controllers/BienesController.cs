using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class BienesController : Controller
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<BienesController> _logger;

  public BienesController(IConfiguration configuration, ILogger<BienesController> logger)
  {
    _configuration = configuration;
    _logger = logger;
  }

  public async Task<ActionResult> GetBienesAprobados(string tableName)
  {
    var connString = _configuration.GetConnectionString("FinanManagerDBConnection");

    if (string.IsNullOrEmpty(connString))
    {
      _logger.LogError("Connection string 'FinanManagerDBConnection' no encontrada");
      ViewData["ErrorMessage"] = "No se pudo establecer conexión con la base de datos";
      return View("Error");
    }

    var dynamicModels = new List<dynamic>();

    try
    {
      await using (var sqlConnection = new SqlConnection(connString))
      {
        await sqlConnection.OpenAsync();

        using (var cmd = new SqlCommand("SP_GetBienesAprobados", sqlConnection))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@table", tableName);

          await using (var reader = await cmd.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              dynamic dynamicModel = new ExpandoObject();

              // Propiedades comunes
              dynamicModel.RoleName = reader["RoleName"].ToString();
              dynamicModel.Fecha = Convert.ToDateTime(reader["Fecha"]);
              dynamicModel.Descripcion = reader["Descripcion"].ToString();

              // Propiedades específicas según la tabla
              switch (tableName)
              {
                case "Bienes":
                  dynamicModel.Cantidad = Convert.ToInt32(reader["Cantidad"]);
                  dynamicModel.MontoUnitario = Convert.ToDecimal(reader["MontoUnitario"]);
                  dynamicModel.Total = Convert.ToDecimal(reader["Total"]);
                  dynamicModel.MotivoRechazo = reader["MotivoRechazo"].ToString();
                  break;

                case "Gasto":
                  dynamicModel.CuentaMadre_ID = reader["CuentaMadre_ID"].ToString();
                  dynamicModel.Justificacion = reader["Descripcion"].ToString();
                  dynamicModel.Total = Convert.ToDecimal(reader["Total"]);
                  break;

                case "Proyectos":
                  dynamicModel.ValorEstimado = Convert.ToDecimal(reader["ValorEstimado"]);
                  dynamicModel.VialidadComercial = reader["viabilidadComercial"].ToString() == "1";
                  dynamicModel.VialidadTecnica = reader["viabilidadTecnica"].ToString() == "1";
                  dynamicModel.VialidadLegal = reader["viabilidadLegal"].ToString() == "1";
                  dynamicModel.VialidadGestion = reader["viabilidadGestion"].ToString() == "1";
                  dynamicModel.VialidadFinanciera = reader["viabilidadFinanciera"].ToString() == "1";
                  break;
              }

              dynamicModels.Add(dynamicModel);
            }
          }
        }
      }


      if (!dynamicModels.Any())
      {
        ViewBag.NoDataMessage = "No hay datos para el departamento seleccionado.";
      }

      ViewBag.TableType = tableName;
      return View(dynamicModels);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error al obtener bienes aprobados");
      ViewData["ErrorMessage"] = $"Error al procesar la solicitud: {ex.Message}";
      return View("Error");
    }
  }


}
