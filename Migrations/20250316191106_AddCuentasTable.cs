using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanManager.Migrations
{
    /// <inheritdoc />
    public partial class AddCuentasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    CuentaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CuentaMadreId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.CuentaId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    role_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__760F9984B55F0625", x => x.role_ID);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesRestablecimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Solicitu__3214EC073E6D05A5", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesRestablecimientoAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Responsable = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Solicitu__3214EC07A94A73ED", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "statusPresupuesto",
                columns: table => new
                {
                    status_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    statusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__statusPr__3680B97982B97846", x => x.status_ID);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    users_ID = table.Column<int>(type: "int", nullable: false),
                    RolAnterior_ID = table.Column<int>(type: "int", nullable: false),
                    NuevoRol_ID = table.Column<int>(type: "int", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__3214EC07D98963ED", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    SpendingLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NearLimitValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NearLimitAlert = table.Column<int>(type: "int", nullable: false),
                    ExceedLimitAlert = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertConfig_Roles",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "role_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Notifications_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notificationType = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    notificationMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__9688F61C9302D258", x => x.Notifications_ID);
                    table.ForeignKey(
                        name: "FK_Notifications_Roles",
                        column: x => x.role_ID,
                        principalTable: "Roles",
                        principalColumn: "role_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    users_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    lastName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    userStatus = table.Column<int>(type: "int", nullable: false),
                    userEmail = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    role_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__EAA0ED73A125BD51", x => x.users_ID);
                    table.ForeignKey(
                        name: "FK_Users_Roles",
                        column: x => x.role_ID,
                        principalTable: "Roles",
                        principalColumn: "role_ID");
                });

            migrationBuilder.CreateTable(
                name: "Bienes",
                columns: table => new
                {
                    bien_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    montoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    enero = table.Column<int>(type: "int", nullable: false),
                    febrero = table.Column<int>(type: "int", nullable: false),
                    marzo = table.Column<int>(type: "int", nullable: false),
                    abril = table.Column<int>(type: "int", nullable: false),
                    mayo = table.Column<int>(type: "int", nullable: false),
                    junio = table.Column<int>(type: "int", nullable: false),
                    julio = table.Column<int>(type: "int", nullable: false),
                    agosto = table.Column<int>(type: "int", nullable: false),
                    septiembre = table.Column<int>(type: "int", nullable: false),
                    octubre = table.Column<int>(type: "int", nullable: false),
                    noviembre = table.Column<int>(type: "int", nullable: false),
                    diciembre = table.Column<int>(type: "int", nullable: false),
                    role_ID = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    status_ID = table.Column<int>(type: "int", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bienes__53E7FB7B0F7E62AF", x => x.bien_ID);
                    table.ForeignKey(
                        name: "FK_Bienes_Roles",
                        column: x => x.role_ID,
                        principalTable: "Roles",
                        principalColumn: "role_ID");
                    table.ForeignKey(
                        name: "FK_Bienes_Status",
                        column: x => x.status_ID,
                        principalTable: "statusPresupuesto",
                        principalColumn: "status_ID");
                });

            migrationBuilder.CreateTable(
                name: "Gasto",
                columns: table => new
                {
                    gasto_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cuentaMadre_ID = table.Column<int>(type: "int", nullable: false),
                    cuentaHija_ID = table.Column<int>(type: "int", nullable: false),
                    justificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    enero = table.Column<int>(type: "int", nullable: false),
                    febrero = table.Column<int>(type: "int", nullable: false),
                    marzo = table.Column<int>(type: "int", nullable: false),
                    abril = table.Column<int>(type: "int", nullable: false),
                    mayo = table.Column<int>(type: "int", nullable: false),
                    junio = table.Column<int>(type: "int", nullable: false),
                    julio = table.Column<int>(type: "int", nullable: false),
                    agosto = table.Column<int>(type: "int", nullable: false),
                    septiembre = table.Column<int>(type: "int", nullable: false),
                    octubre = table.Column<int>(type: "int", nullable: false),
                    noviembre = table.Column<int>(type: "int", nullable: false),
                    diciembre = table.Column<int>(type: "int", nullable: false),
                    role_ID = table.Column<int>(type: "int", nullable: false),
                    status_ID = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Gasto__5F5C0E732B01BED9", x => x.gasto_ID);
                    table.ForeignKey(
                        name: "FK_Gasto_Roles",
                        column: x => x.role_ID,
                        principalTable: "Roles",
                        principalColumn: "role_ID");
                    table.ForeignKey(
                        name: "FK_Gasto_Status",
                        column: x => x.status_ID,
                        principalTable: "statusPresupuesto",
                        principalColumn: "status_ID");
                });

            migrationBuilder.CreateTable(
                name: "Proyectos",
                columns: table => new
                {
                    proyecto_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    valorEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    viabilidadComercial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    viabilidadTecnica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    viabilidadLegal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    viabilidadGestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    viabilidadImpactoAmbiental = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    viabilidadFinanciera = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role_ID = table.Column<int>(type: "int", nullable: false),
                    status_ID = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Proyecto__A7383809345897B9", x => x.proyecto_ID);
                    table.ForeignKey(
                        name: "FK_Proyectos_Roles",
                        column: x => x.role_ID,
                        principalTable: "Roles",
                        principalColumn: "role_ID");
                    table.ForeignKey(
                        name: "FK_Proyectos_Status",
                        column: x => x.status_ID,
                        principalTable: "statusPresupuesto",
                        principalColumn: "status_ID");
                });

            migrationBuilder.CreateTable(
                name: "Accounting",
                columns: table => new
                {
                    accounting_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    accountName = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    debitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    creditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    transactionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    createdBy = table.Column<int>(type: "int", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    isReconciled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Accounti__19BA3200DED7644F", x => x.accounting_ID);
                    table.ForeignKey(
                        name: "FK__Accountin__creat__2E1BDC42",
                        column: x => x.createdBy,
                        principalTable: "Users",
                        principalColumn: "users_ID");
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    audit_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tableName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    recordId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    operation = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    oldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    newValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    changedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: false),
                    ipAddress = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    sessionId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    applicationName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    reason = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditLog__5AF23A3B1B80755A", x => x.audit_ID);
                    table.ForeignKey(
                        name: "FK__AuditLog__userId__2F10007B",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "users_ID");
                });

            migrationBuilder.CreateTable(
                name: "FailedLoginAttempts",
                columns: table => new
                {
                    attempt_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_ID = table.Column<int>(type: "int", nullable: false),
                    userEmail = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    attemptDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FailedLo__5620F57154470F1C", x => x.attempt_ID);
                    table.ForeignKey(
                        name: "FK_FailedLoginAttempts_Users",
                        column: x => x.user_ID,
                        principalTable: "Users",
                        principalColumn: "users_ID");
                });

            migrationBuilder.CreateTable(
                name: "Presupuesto",
                columns: table => new
                {
                    PresupuestoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByNavigationUsers_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuesto", x => x.PresupuestoId);
                    table.ForeignKey(
                        name: "FK_Presupuesto_Users_CreatedByNavigationUsers_Id",
                        column: x => x.CreatedByNavigationUsers_Id,
                        principalTable: "Users",
                        principalColumn: "users_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounting_createdBy",
                table: "Accounting",
                column: "createdBy");

            migrationBuilder.CreateIndex(
                name: "IX_AlertConfig_RoleId",
                table: "AlertConfig",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_userId",
                table: "AuditLog",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Bienes_role_ID",
                table: "Bienes",
                column: "role_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Bienes_status_ID",
                table: "Bienes",
                column: "status_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FailedLoginAttempts_user_ID",
                table: "FailedLoginAttempts",
                column: "user_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_role_ID",
                table: "Gasto",
                column: "role_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Gasto_status_ID",
                table: "Gasto",
                column: "status_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_role_ID",
                table: "Notifications",
                column: "role_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_CreatedByNavigationUsers_Id",
                table: "Presupuesto",
                column: "CreatedByNavigationUsers_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_role_ID",
                table: "Proyectos",
                column: "role_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_status_ID",
                table: "Proyectos",
                column: "status_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_role_ID",
                table: "Users",
                column: "role_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounting");

            migrationBuilder.DropTable(
                name: "AlertConfig");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "Bienes");

            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "FailedLoginAttempts");

            migrationBuilder.DropTable(
                name: "Gasto");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Presupuesto");

            migrationBuilder.DropTable(
                name: "Proyectos");

            migrationBuilder.DropTable(
                name: "SolicitudesRestablecimiento");

            migrationBuilder.DropTable(
                name: "SolicitudesRestablecimientoAdmin");

            migrationBuilder.DropTable(
                name: "UserRoleChanges");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "statusPresupuesto");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
