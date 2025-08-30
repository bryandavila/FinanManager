using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class UserRoleChange
{
    public int Id { get; set; }

    public int UsersId { get; set; }

    public int RolAnteriorId { get; set; }

    public int NuevoRolId { get; set; }

    public DateTime FechaCambio { get; set; }

    public string Responsable { get; set; } = null!;
}
