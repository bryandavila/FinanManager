using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class SolicitudesRestablecimientoAdmin
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Responsable { get; set; } = null!;
}
