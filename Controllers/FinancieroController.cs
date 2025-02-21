using Microsoft.AspNetCore.Mvc;

namespace FinanManager.Controllers;
public class FinancieroController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult DetalleNotificacionView()
    {
      return View();
    }
}
