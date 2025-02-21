using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class SolicitudesRestablecimiento
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime FechaExpiracion { get; set; }
}
