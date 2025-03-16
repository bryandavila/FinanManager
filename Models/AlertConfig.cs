using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanManager.Models
{
  public class AlertConfig
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int RoleId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal SpendingLimit { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal NearLimitValue { get; set; }

    [Required]
    public int NearLimitAlert { get; set; }

    [Required]
    public int ExceedLimitAlert { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }

    // Relación con la tabla Roles
    [ForeignKey("RoleId")]
    public Role? Role { get; set; }
  }
}
