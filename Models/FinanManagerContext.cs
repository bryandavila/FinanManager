using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinanManager.Models;

public partial class FinanManagerContext : DbContext
{
  public FinanManagerContext()
  {
  }

  public FinanManagerContext(DbContextOptions<FinanManagerContext> options)
      : base(options)
  {
  }

  public virtual DbSet<Accounting> Accountings { get; set; }

  public virtual DbSet<AuditLog> AuditLogs { get; set; }

  public virtual DbSet<Bienes> Bienes { get; set; }

  public virtual DbSet<FailedLoginAttempt> FailedLoginAttempts { get; set; }

  public virtual DbSet<Gasto> Gasto { get; set; }

  public virtual DbSet<Notification> Notifications { get; set; }

  public virtual DbSet<Proyecto> Proyectos { get; set; }

  public virtual DbSet<Role> Roles { get; set; }

  public virtual DbSet<SolicitudesRestablecimiento> SolicitudesRestablecimientos { get; set; }

  public virtual DbSet<SolicitudesRestablecimientoAdmin> SolicitudesRestablecimientoAdmins { get; set; }

  public virtual DbSet<StatusPresupuesto> StatusPresupuestos { get; set; }

  public virtual DbSet<User> Users { get; set; }

  public virtual DbSet<UserRoleChange> UserRoleChanges { get; set; }

  public virtual DbSet<AlertConfig> AlertConfigs { get; set; }

  public DbSet<BienesAprobados> BienesAprobado { get; set; }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Accounting>(entity =>
    {
      entity.HasKey(e => e.AccountingId).HasName("PK__Accounti__19BA3200DED7644F");

      entity.ToTable("Accounting");

      entity.Property(e => e.AccountingId).HasColumnName("accounting_ID");
      entity.Property(e => e.AccountName)
              .HasMaxLength(150)
              .IsUnicode(false)
              .HasColumnName("accountName");
      entity.Property(e => e.AccountNumber)
              .HasMaxLength(50)
              .IsUnicode(false)
              .HasColumnName("accountNumber");
      entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
      entity.Property(e => e.CreatedDate)
              .HasDefaultValueSql("(getdate())")
              .HasColumnType("datetime")
              .HasColumnName("createdDate");
      entity.Property(e => e.CreditAmount)
              .HasColumnType("decimal(18, 2)")
              .HasColumnName("creditAmount");
      entity.Property(e => e.DebitAmount)
              .HasColumnType("decimal(18, 2)")
              .HasColumnName("debitAmount");
      entity.Property(e => e.Description).HasColumnName("description");
      entity.Property(e => e.IsReconciled).HasColumnName("isReconciled");
      entity.Property(e => e.TransactionDate)
              .HasColumnType("datetime")
              .HasColumnName("transactionDate");

      entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Accountings)
              .HasForeignKey(d => d.CreatedBy)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__Accountin__creat__2E1BDC42");
    });

    modelBuilder.Entity<AuditLog>(entity =>
    {
      entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__5AF23A3B1B80755A");

      entity.ToTable("AuditLog");

      entity.Property(e => e.AuditId).HasColumnName("audit_ID");
      entity.Property(e => e.ApplicationName)
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasColumnName("applicationName");
      entity.Property(e => e.ChangedColumns).HasColumnName("changedColumns");
      entity.Property(e => e.IpAddress)
              .HasMaxLength(45)
              .IsUnicode(false)
              .HasColumnName("ipAddress");
      entity.Property(e => e.NewValues).HasColumnName("newValues");
      entity.Property(e => e.OldValues).HasColumnName("oldValues");
      entity.Property(e => e.Operation)
              .HasMaxLength(10)
              .IsUnicode(false)
              .HasColumnName("operation");
      entity.Property(e => e.Reason)
              .HasMaxLength(500)
              .IsUnicode(false)
              .HasColumnName("reason");
      entity.Property(e => e.RecordId)
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasColumnName("recordId");
      entity.Property(e => e.SessionId)
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasColumnName("sessionId");
      entity.Property(e => e.TableName)
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasColumnName("tableName");
      entity.Property(e => e.Timestamp)
              .HasDefaultValueSql("(getutcdate())")
              .HasColumnType("datetime")
              .HasColumnName("timestamp");
      entity.Property(e => e.UserId).HasColumnName("userId");

      entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
              .HasForeignKey(d => d.UserId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__AuditLog__userId__2F10007B");
    });

    modelBuilder.Entity<Bienes>(entity =>
    {
      entity.HasKey(e => e.BienId).HasName("PK__Bienes__53E7FB7B0F7E62AF");

      entity.Property(e => e.BienId).HasColumnName("bien_ID");
      entity.Property(e => e.Abril).HasColumnName("abril");
      entity.Property(e => e.Agosto).HasColumnName("agosto");
      entity.Property(e => e.Cantidad).HasColumnName("cantidad");
      entity.Property(e => e.Descripcion).HasColumnName("descripcion");
      entity.Property(e => e.Diciembre).HasColumnName("diciembre");
      entity.Property(e => e.Enero).HasColumnName("enero");
      entity.Property(e => e.Febrero).HasColumnName("febrero");
      entity.Property(e => e.Fecha)
          .HasDefaultValueSql("(getdate())")
          .HasColumnType("datetime")
          .HasColumnName("fecha");
      entity.Property(e => e.Julio).HasColumnName("julio");
      entity.Property(e => e.Junio).HasColumnName("junio");
      entity.Property(e => e.Marzo).HasColumnName("marzo");
      entity.Property(e => e.Mayo).HasColumnName("mayo");
      entity.Property(e => e.MontoUnitario)
          .HasColumnType("decimal(18, 2)")
          .HasColumnName("montoUnitario");
      entity.Property(e => e.Noviembre).HasColumnName("noviembre");
      entity.Property(e => e.Octubre).HasColumnName("octubre");
      entity.Property(e => e.RoleId).HasColumnName("role_ID");
      entity.Property(e => e.Septiembre).HasColumnName("septiembre");
      entity.Property(e => e.StatusId).HasColumnName("status_ID"); // Usa StatusId
      entity.Property(e => e.Total)
          .HasColumnType("decimal(18, 2)")
          .HasColumnName("total");

      // Configuración de la relación con Role
      entity.HasOne(d => d.Role)
          .WithMany(p => p.Bienes)
          .HasForeignKey(d => d.RoleId)
          .OnDelete(DeleteBehavior.ClientSetNull)
          .HasConstraintName("FK_Bienes_Roles");

      // Configuración de la relación con StatusPresupuesto
      entity.HasOne(d => d.StatusPresupuesto) // Usa la propiedad de navegación correcta
          .WithMany(p => p.Bienes)
          .HasForeignKey(d => d.StatusId) // Usa la clave externa correcta
          .OnDelete(DeleteBehavior.ClientSetNull)
          .HasConstraintName("FK_Bienes_Status");
    });

    modelBuilder.Entity<FailedLoginAttempt>(entity =>
        {
          entity.HasKey(e => e.AttemptId).HasName("PK__FailedLo__5620F57154470F1C");

          entity.Property(e => e.AttemptId).HasColumnName("attempt_ID");
          entity.Property(e => e.AttemptDate)
              .HasDefaultValueSql("(getdate())")
              .HasColumnName("attemptDate");
          entity.Property(e => e.UserEmail)
              .HasMaxLength(255)
              .IsUnicode(false)
              .HasColumnName("userEmail");
          entity.Property(e => e.UserId).HasColumnName("user_ID");

          entity.HasOne(d => d.User).WithMany(p => p.FailedLoginAttempts)
              .HasForeignKey(d => d.UserId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_FailedLoginAttempts_Users");
        });

    modelBuilder.Entity<Gasto>(entity =>
    {
      entity.HasKey(e => e.GastoId).HasName("PK__Gasto__5F5C0E732B01BED9");

      entity.ToTable("Gasto");

      entity.Property(e => e.GastoId).HasColumnName("gasto_ID");
      entity.Property(e => e.Abril).HasColumnName("abril");
      entity.Property(e => e.Agosto).HasColumnName("agosto");
      entity.Property(e => e.CuentaHijaId).HasColumnName("cuentaHija_ID");
      entity.Property(e => e.CuentaMadreId).HasColumnName("cuentaMadre_ID");
      entity.Property(e => e.Diciembre).HasColumnName("diciembre");
      entity.Property(e => e.Enero).HasColumnName("enero");
      entity.Property(e => e.Febrero).HasColumnName("febrero");
      entity.Property(e => e.Fecha)
              .HasDefaultValueSql("(getdate())")
              .HasColumnType("datetime")
              .HasColumnName("fecha");
      entity.Property(e => e.Julio).HasColumnName("julio");
      entity.Property(e => e.Junio).HasColumnName("junio");
      entity.Property(e => e.Justificacion).HasColumnName("justificacion");
      entity.Property(e => e.Marzo).HasColumnName("marzo");
      entity.Property(e => e.Mayo).HasColumnName("mayo");
      entity.Property(e => e.Noviembre).HasColumnName("noviembre");
      entity.Property(e => e.Octubre).HasColumnName("octubre");
      entity.Property(e => e.RoleId).HasColumnName("role_ID");
      entity.Property(e => e.Septiembre).HasColumnName("septiembre");
      entity.Property(e => e.StatusId).HasColumnName("status_ID");
      entity.Property(e => e.Total)
              .HasColumnType("decimal(18, 2)")
              .HasColumnName("total");

      entity.HasOne(d => d.Role).WithMany(p => p.Gastos)
              .HasForeignKey(d => d.RoleId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Gasto_Roles");

      entity.HasOne(d => d.Status).WithMany(p => p.Gastos)
              .HasForeignKey(d => d.StatusId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Gasto_Status");
    });

    modelBuilder.Entity<Notification>(entity =>
    {
      entity.HasKey(e => e.NotificationsId).HasName("PK__Notifica__9688F61C9302D258");

      entity.Property(e => e.NotificationsId).HasColumnName("Notifications_ID");
      entity.Property(e => e.NotificationMessage).HasColumnName("notificationMessage");
      entity.Property(e => e.NotificationType)
          .HasMaxLength(500)
          .IsUnicode(false)
          .HasColumnName("notificationType");
      entity.Property(e => e.role_ID).HasColumnName("role_ID"); // Mapea role_ID

      // Configuración de la relación con la tabla Roles
      entity.HasOne(d => d.Role)
          .WithMany(p => p.Notifications)
          .HasForeignKey(d => d.role_ID)
          .HasConstraintName("FK_Notifications_Roles");
    });

    modelBuilder.Entity<Proyecto>(entity =>
        {
          entity.HasKey(e => e.ProyectoId).HasName("PK__Proyecto__A7383809345897B9");

          entity.Property(e => e.ProyectoId).HasColumnName("proyecto_ID");
          entity.Property(e => e.Fecha)
              .HasDefaultValueSql("(getdate())")
              .HasColumnType("datetime")
              .HasColumnName("fecha");
          entity.Property(e => e.RoleId).HasColumnName("role_ID");
          entity.Property(e => e.StatusId).HasColumnName("status_ID");
          entity.Property(e => e.ValorEstimado)
              .HasColumnType("decimal(18, 2)")
              .HasColumnName("valorEstimado");
          entity.Property(e => e.Descripcion).HasColumnName("descripcion");
          entity.Property(e => e.ViabilidadComercial).HasColumnName("viabilidadComercial");
          entity.Property(e => e.ViabilidadFinanciera).HasColumnName("viabilidadFinanciera");
          entity.Property(e => e.ViabilidadGestion).HasColumnName("viabilidadGestion");
          entity.Property(e => e.ViabilidadImpactoAmbiental).HasColumnName("viabilidadImpactoAmbiental");
          entity.Property(e => e.ViabilidadLegal).HasColumnName("viabilidadLegal");
          entity.Property(e => e.ViabilidadTecnica).HasColumnName("viabilidadTecnica");

          entity.HasOne(d => d.Role).WithMany(p => p.Proyectos)
              .HasForeignKey(d => d.RoleId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Proyectos_Roles");

          entity.HasOne(d => d.Status).WithMany(p => p.Proyectos)
              .HasForeignKey(d => d.StatusId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Proyectos_Status");
        });

    modelBuilder.Entity<Role>(entity =>
    {
      entity.HasKey(e => e.role_ID).HasName("PK__Roles__760F9984B55F0625");

      entity.Property(e => e.role_ID).HasColumnName("role_ID");
      entity.Property(e => e.role_name)
              .HasMaxLength(150)
              .IsUnicode(false)
              .HasColumnName("role_name");
    });

    modelBuilder.Entity<SolicitudesRestablecimiento>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK__Solicitu__3214EC073E6D05A5");

      entity.ToTable("SolicitudesRestablecimiento");

      entity.Property(e => e.Email).HasMaxLength(256);
      entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
      entity.Property(e => e.Token).HasMaxLength(256);
    });

    modelBuilder.Entity<SolicitudesRestablecimientoAdmin>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK__Solicitu__3214EC07A94A73ED");

      entity.ToTable("SolicitudesRestablecimientoAdmin");

      entity.Property(e => e.CreatedDate)
              .HasDefaultValueSql("(getdate())")
              .HasColumnType("datetime");
      entity.Property(e => e.Email).HasMaxLength(256);
      entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
      entity.Property(e => e.Responsable).HasMaxLength(256);
      entity.Property(e => e.Token).HasMaxLength(256);
    });

    modelBuilder.Entity<StatusPresupuesto>(entity =>
    {
      entity.HasKey(e => e.StatusId).HasName("PK__statusPr__3680B97982B97846");

      entity.ToTable("statusPresupuesto");

      entity.Property(e => e.StatusId).HasColumnName("status_ID");
      entity.Property(e => e.StatusName)
              .HasMaxLength(50)
              .HasColumnName("statusName");
    });

    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Users_Id).HasName("PK__Users__EAA0ED73A125BD51");

      entity.Property(e => e.Users_Id).HasColumnName("users_ID");
      entity.Property(e => e.CreatedDate)
              .HasDefaultValueSql("(getdate())")
              .HasColumnName("createdDate");
      entity.Property(e => e.LastName)
              .HasMaxLength(150)
              .IsUnicode(false)
              .HasColumnName("lastName");
      entity.Property(e => e.Name)
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasColumnName("name");
      entity.Property(e => e.Password)
              .HasMaxLength(255)
              .HasColumnName("password");
      entity.Property(e => e.role_ID).HasColumnName("role_ID");
      entity.Property(e => e.UserEmail)
              .HasMaxLength(255)
              .IsUnicode(false)
              .HasColumnName("userEmail");
      entity.Property(e => e.UserStatus).HasColumnName("userStatus");

      entity.HasOne(d => d.Role).WithMany(p => p.Users)
              .HasForeignKey(d => d.role_ID)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK_Users_Roles");
    });

    modelBuilder.Entity<UserRoleChange>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07D98963ED");

      entity.Property(e => e.FechaCambio).HasColumnType("datetime");
      entity.Property(e => e.NuevoRolId).HasColumnName("NuevoRol_ID");
      entity.Property(e => e.Responsable).HasMaxLength(256);
      entity.Property(e => e.RolAnteriorId).HasColumnName("RolAnterior_ID");
      entity.Property(e => e.UsersId).HasColumnName("users_ID");
    });

    modelBuilder.Entity<AlertConfig>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK_AlertConfig");

      entity.ToTable("AlertConfig");

      entity.Property(e => e.Id).HasColumnName("Id");
      entity.Property(e => e.RoleId).HasColumnName("RoleId");
      entity.Property(e => e.SpendingLimit).HasColumnType("decimal(18, 2)");
      entity.Property(e => e.NearLimitValue).HasColumnType("decimal(18, 2)");
      entity.Property(e => e.NearLimitAlert).IsRequired();
      entity.Property(e => e.ExceedLimitAlert).IsRequired();
      entity.Property(e => e.CreatedAt)
          .HasDefaultValueSql("GETDATE()")
          .ValueGeneratedOnAdd();
      entity.Property(e => e.UpdatedAt)
          .HasDefaultValueSql("GETDATE()")
          .ValueGeneratedOnAddOrUpdate();

      // Relación con la tabla Roles
      entity.HasOne(a => a.Role)
          .WithMany(r => r.AlertConfigs)
          .HasForeignKey(a => a.RoleId)
          .OnDelete(DeleteBehavior.Cascade)
          .HasConstraintName("FK_AlertConfig_Roles");
    });

    modelBuilder.Entity<BienesAprobados>(entity =>
    {
      entity.HasNoKey(); // Especifica que es una entidad sin clave

      // Configuraciones opcionales de columnas si es necesario
      entity.Property(e => e.Descripcion).HasColumnName("descripcion");
      entity.Property(e => e.Cantidad).HasColumnName("cantidad");
      entity.Property(e => e.MontoUnitario).HasColumnName("montoUnitario");
      entity.Property(e => e.Total).HasColumnName("total");
      entity.Property(e => e.MotivoRechazo).HasColumnName("MotivoRechazo");
      entity.Property(e => e.Fecha).HasColumnName("fecha");
     // entity.Property(e => e.RoleName).HasColumnName("role_name");
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
