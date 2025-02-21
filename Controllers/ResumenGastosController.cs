using Microsoft.AspNetCore.Mvc;

namespace FinanManager.Controllers;
public class ResumenGastosController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

  }
