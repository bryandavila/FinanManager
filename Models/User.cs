using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanManager.Models;

public partial class User
{
  [Key]
  [Column("users_ID")] // Especifica el nombre de la columna en la base de datos
  public int Users_Id { get; set; }

  [Required(ErrorMessage = "El nombre es obligatorio")]
  public string Name { get; set; } = null!;

  [Required(ErrorMessage = "El apellido es obligatorio")]
  public string LastName { get; set; } = null!;

  [Required(ErrorMessage = "La contraseña es obligatoria")]
  public string Password { get; set; } = null!;

  public DateTime? CreatedDate { get; set; }

  public int UserStatus { get; set; }

  [Required(ErrorMessage = "El correo electrónico es obligatorio")]
  [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
  public string UserEmail { get; set; } = null!;

  [Required(ErrorMessage = "El rol es obligatorio")]
  public int role_ID { get; set; }

  public virtual ICollection<Accounting> Accountings { get; set; } = new List<Accounting>();

  public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

  public virtual ICollection<FailedLoginAttempt> FailedLoginAttempts { get; set; } = new List<FailedLoginAttempt>();

  public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();

  public virtual Role Role { get; set; } = null!;
}
