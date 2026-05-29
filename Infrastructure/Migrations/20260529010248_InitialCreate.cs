using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasRepuesto",
                columns: table => new
                {
                    IdCategoriaRepuesto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Activo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false, defaultValue: "True"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasRepuesto", x => x.IdCategoriaRepuesto);
                });

            migrationBuilder.CreateTable(
                name: "CodigosTelefono",
                columns: table => new
                {
                    IdCodigoTelefono = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Pais = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodigosTelefono", x => x.IdCodigoTelefono);
                });

            migrationBuilder.CreateTable(
                name: "ColoresVehiculo",
                columns: table => new
                {
                    IdColor = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreColor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColoresVehiculo", x => x.IdColor);
                });

            migrationBuilder.CreateTable(
                name: "DominiosCorreo",
                columns: table => new
                {
                    IdDominioCorreo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Dominio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DominiosCorreo", x => x.IdDominioCorreo);
                });

            migrationBuilder.CreateTable(
                name: "EstadosOrden",
                columns: table => new
                {
                    IdEstadoOrden = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EsFinal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Descripcion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosOrden", x => x.IdEstadoOrden);
                });

            migrationBuilder.CreateTable(
                name: "MarcasVehiculo",
                columns: table => new
                {
                    IdMarca = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreMarca = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarcasVehiculo", x => x.IdMarca);
                });

            migrationBuilder.CreateTable(
                name: "MetodosPago",
                columns: table => new
                {
                    IdMetodoPago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodosPago", x => x.IdMetodoPago);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "DATE", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.IdPersona);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Talleres",
                columns: table => new
                {
                    IdTaller = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Nit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    RazonSocial = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Ciudad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talleres", x => x.IdTaller);
                });

            migrationBuilder.CreateTable(
                name: "TiposAccionAuditoria",
                columns: table => new
                {
                    IdTipoAccionAuditoria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAccionAuditoria", x => x.IdTipoAccionAuditoria);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumentos",
                columns: table => new
                {
                    IdTipoDocumento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumentos", x => x.IdTipoDocumento);
                });

            migrationBuilder.CreateTable(
                name: "TiposServicio",
                columns: table => new
                {
                    IdTipoServicio = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DuracionEstimadaHoras = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    PrecioPorHora = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposServicio", x => x.IdTipoServicio);
                });

            migrationBuilder.CreateTable(
                name: "ModelosVehiculo",
                columns: table => new
                {
                    IdModelo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdMarca = table.Column<int>(type: "integer", nullable: false),
                    NombreModelo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosVehiculo", x => x.IdModelo);
                    table.ForeignKey(
                        name: "FK_ModelosVehiculo_MarcasVehiculo_IdMarca",
                        column: x => x.IdMarca,
                        principalTable: "MarcasVehiculo",
                        principalColumn: "IdMarca",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonasCorreos",
                columns: table => new
                {
                    IdPersonaCorreo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    IdDominioCorreo = table.Column<int>(type: "integer", nullable: false),
                    UsuarioCorreo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonasCorreos", x => x.IdPersonaCorreo);
                    table.ForeignKey(
                        name: "FK_PersonasCorreos_DominiosCorreo_IdDominioCorreo",
                        column: x => x.IdDominioCorreo,
                        principalTable: "DominiosCorreo",
                        principalColumn: "IdDominioCorreo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonasCorreos_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonasTelefonos",
                columns: table => new
                {
                    IdPersonaTelefono = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    IdCodigoTelefono = table.Column<int>(type: "integer", nullable: false),
                    NumeroTelefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonasTelefonos", x => x.IdPersonaTelefono);
                    table.ForeignKey(
                        name: "FK_PersonasTelefonos_CodigosTelefono_IdCodigoTelefono",
                        column: x => x.IdCodigoTelefono,
                        principalTable: "CodigosTelefono",
                        principalColumn: "IdCodigoTelefono",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonasTelefonos_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTaller = table.Column<int>(type: "integer", nullable: false),
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    DireccionCalle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DireccionCiudad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    DireccionEstado = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    DireccionCodigoPostal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                    table.ForeignKey(
                        name: "FK_Clientes_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_Talleres_IdTaller",
                        column: x => x.IdTaller,
                        principalTable: "Talleres",
                        principalColumn: "IdTaller",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Repuestos",
                columns: table => new
                {
                    IdRepuesto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTaller = table.Column<int>(type: "integer", nullable: false),
                    IdCategoriaRepuesto = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MarcaRepuesto = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UbicacionAlmacen = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StockActual = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    StockMinimo = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repuestos", x => x.IdRepuesto);
                    table.ForeignKey(
                        name: "FK_Repuestos_CategoriasRepuesto_IdCategoriaRepuesto",
                        column: x => x.IdCategoriaRepuesto,
                        principalTable: "CategoriasRepuesto",
                        principalColumn: "IdCategoriaRepuesto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Repuestos_Talleres_IdTaller",
                        column: x => x.IdTaller,
                        principalTable: "Talleres",
                        principalColumn: "IdTaller",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTaller = table.Column<int>(type: "integer", nullable: false),
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    ContraseñaHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RefreshToken = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FechaExpiracionRefreshToken = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaUltimoLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Talleres_IdTaller",
                        column: x => x.IdTaller,
                        principalTable: "Talleres",
                        principalColumn: "IdTaller",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonasDocumentos",
                columns: table => new
                {
                    IdPersonaDocumento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPersona = table.Column<int>(type: "integer", nullable: false),
                    IdTipoDocumento = table.Column<int>(type: "integer", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonasDocumentos", x => x.IdPersonaDocumento);
                    table.ForeignKey(
                        name: "FK_PersonasDocumentos_Personas_IdPersona",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "IdPersona",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonasDocumentos_TiposDocumentos_IdTipoDocumento",
                        column: x => x.IdTipoDocumento,
                        principalTable: "TiposDocumentos",
                        principalColumn: "IdTipoDocumento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                columns: table => new
                {
                    IdVehiculo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTaller = table.Column<int>(type: "integer", nullable: false),
                    IdModelo = table.Column<int>(type: "integer", nullable: false),
                    IdColor = table.Column<int>(type: "integer", nullable: false),
                    Placa = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    VIN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    Kilometraje = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.IdVehiculo);
                    table.ForeignKey(
                        name: "FK_Vehiculos_ColoresVehiculo_IdColor",
                        column: x => x.IdColor,
                        principalTable: "ColoresVehiculo",
                        principalColumn: "IdColor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehiculos_ModelosVehiculo_IdModelo",
                        column: x => x.IdModelo,
                        principalTable: "ModelosVehiculo",
                        principalColumn: "IdModelo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehiculos_Talleres_IdTaller",
                        column: x => x.IdTaller,
                        principalTable: "Talleres",
                        principalColumn: "IdTaller",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    IdAuditoria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTaller = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdTipoAccionAuditoria = table.Column<int>(type: "integer", nullable: false),
                    EntidadAfectada = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdRegistroAfectado = table.Column<int>(type: "integer", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ValoresAnteriores = table.Column<string>(type: "JSONB", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "JSONB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.IdAuditoria);
                    table.ForeignKey(
                        name: "FK_Auditorias_Talleres_IdTaller",
                        column: x => x.IdTaller,
                        principalTable: "Talleres",
                        principalColumn: "IdTaller",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auditorias_TiposAccionAuditoria_IdTipoAccionAuditoria",
                        column: x => x.IdTipoAccionAuditoria,
                        principalTable: "TiposAccionAuditoria",
                        principalColumn: "IdTipoAccionAuditoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auditorias_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosRoles",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosRoles", x => new { x.IdUsuario, x.IdRol });
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialDuenosVehiculo",
                columns: table => new
                {
                    IdHistorial = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdVehiculo = table.Column<int>(type: "integer", nullable: false),
                    IdCliente = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "DATE", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "DATE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialDuenosVehiculo", x => x.IdHistorial);
                    table.ForeignKey(
                        name: "FK_HistorialDuenosVehiculo_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialDuenosVehiculo_Vehiculos_IdVehiculo",
                        column: x => x.IdVehiculo,
                        principalTable: "Vehiculos",
                        principalColumn: "IdVehiculo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesServicio",
                columns: table => new
                {
                    IdOrdenServicio = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTaller = table.Column<int>(type: "integer", nullable: false),
                    IdVehiculo = table.Column<int>(type: "integer", nullable: false),
                    IdTipoServicio = table.Column<int>(type: "integer", nullable: false),
                    IdMecanico = table.Column<int>(type: "integer", nullable: false),
                    IdRecepcionista = table.Column<int>(type: "integer", nullable: true),
                    IdEstadoOrden = table.Column<int>(type: "integer", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaEntregaEstimada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaEntregaReal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TrabajoRealizado = table.Column<string>(type: "TEXT", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesServicio", x => x.IdOrdenServicio);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_EstadosOrden_IdEstadoOrden",
                        column: x => x.IdEstadoOrden,
                        principalTable: "EstadosOrden",
                        principalColumn: "IdEstadoOrden",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Talleres_IdTaller",
                        column: x => x.IdTaller,
                        principalTable: "Talleres",
                        principalColumn: "IdTaller",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_TiposServicio_IdTipoServicio",
                        column: x => x.IdTipoServicio,
                        principalTable: "TiposServicio",
                        principalColumn: "IdTipoServicio",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Usuarios_IdMecanico",
                        column: x => x.IdMecanico,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Usuarios_IdRecepcionista",
                        column: x => x.IdRecepcionista,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Vehiculos_IdVehiculo",
                        column: x => x.IdVehiculo,
                        principalTable: "Vehiculos",
                        principalColumn: "IdVehiculo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    IdFactura = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTaller = table.Column<int>(type: "integer", nullable: false),
                    IdOrdenServicio = table.Column<int>(type: "integer", nullable: false),
                    IdMetodoPago = table.Column<int>(type: "integer", nullable: false),
                    NumeroFactura = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstadoPago = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pendiente"),
                    CostoManoObra = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    Subtotal = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Impuestos = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    Total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.IdFactura);
                    table.ForeignKey(
                        name: "FK_Facturas_MetodosPago_IdMetodoPago",
                        column: x => x.IdMetodoPago,
                        principalTable: "MetodosPago",
                        principalColumn: "IdMetodoPago",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facturas_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Facturas_Talleres_IdTaller",
                        column: x => x.IdTaller,
                        principalTable: "Talleres",
                        principalColumn: "IdTaller",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesServicio_Repuestos",
                columns: table => new
                {
                    IdOrdenRepuesto = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrdenServicio = table.Column<int>(type: "integer", nullable: false),
                    IdRepuesto = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioUnitarioAplicado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesServicio_Repuestos", x => x.IdOrdenRepuesto);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Repuestos_OrdenesServicio_IdOrdenServicio",
                        column: x => x.IdOrdenServicio,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrdenServicio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Repuestos_Repuestos_IdRepuesto",
                        column: x => x.IdRepuesto,
                        principalTable: "Repuestos",
                        principalColumn: "IdRepuesto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesFactura",
                columns: table => new
                {
                    IdDetalleFactura = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdFactura = table.Column<int>(type: "integer", nullable: false),
                    Concepto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesFactura", x => x.IdDetalleFactura);
                    table.ForeignKey(
                        name: "FK_DetallesFactura_Facturas_IdFactura",
                        column: x => x.IdFactura,
                        principalTable: "Facturas",
                        principalColumn: "IdFactura",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_IdTaller",
                table: "Auditorias",
                column: "IdTaller");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_IdTipoAccionAuditoria",
                table: "Auditorias",
                column: "IdTipoAccionAuditoria");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_IdUsuario",
                table: "Auditorias",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasRepuesto_Activo",
                table: "CategoriasRepuesto",
                column: "Activo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdPersona",
                table: "Clientes",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdTaller_IdPersona",
                table: "Clientes",
                columns: new[] { "IdTaller", "IdPersona" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodigosTelefono_Codigo",
                table: "CodigosTelefono",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColoresVehiculo_NombreColor",
                table: "ColoresVehiculo",
                column: "NombreColor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesFactura_IdFactura",
                table: "DetallesFactura",
                column: "IdFactura");

            migrationBuilder.CreateIndex(
                name: "IX_DominiosCorreo_Dominio",
                table: "DominiosCorreo",
                column: "Dominio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosOrden_Nombre",
                table: "EstadosOrden",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdMetodoPago",
                table: "Facturas",
                column: "IdMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdOrdenServicio",
                table: "Facturas",
                column: "IdOrdenServicio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_IdTaller_NumeroFactura",
                table: "Facturas",
                columns: new[] { "IdTaller", "NumeroFactura" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialDuenosVehiculo_IdCliente",
                table: "HistorialDuenosVehiculo",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialDuenosVehiculo_IdVehiculo",
                table: "HistorialDuenosVehiculo",
                column: "IdVehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_MarcasVehiculo_NombreMarca",
                table: "MarcasVehiculo",
                column: "NombreMarca",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetodosPago_Nombre",
                table: "MetodosPago",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelosVehiculo_IdMarca_NombreModelo",
                table: "ModelosVehiculo",
                columns: new[] { "IdMarca", "NombreModelo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdEstadoOrden",
                table: "OrdenesServicio",
                column: "IdEstadoOrden");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdMecanico",
                table: "OrdenesServicio",
                column: "IdMecanico");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdRecepcionista",
                table: "OrdenesServicio",
                column: "IdRecepcionista");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdTaller",
                table: "OrdenesServicio",
                column: "IdTaller");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdTipoServicio",
                table: "OrdenesServicio",
                column: "IdTipoServicio");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_IdVehiculo",
                table: "OrdenesServicio",
                column: "IdVehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_Repuestos_IdOrdenServicio",
                table: "OrdenesServicio_Repuestos",
                column: "IdOrdenServicio");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_Repuestos_IdRepuesto",
                table: "OrdenesServicio_Repuestos",
                column: "IdRepuesto");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasCorreos_IdDominioCorreo",
                table: "PersonasCorreos",
                column: "IdDominioCorreo");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasCorreos_IdPersona_IdDominioCorreo_UsuarioCorreo",
                table: "PersonasCorreos",
                columns: new[] { "IdPersona", "IdDominioCorreo", "UsuarioCorreo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonasDocumentos_IdPersona_IdTipoDocumento",
                table: "PersonasDocumentos",
                columns: new[] { "IdPersona", "IdTipoDocumento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonasDocumentos_IdTipoDocumento",
                table: "PersonasDocumentos",
                column: "IdTipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasDocumentos_NumeroDocumento",
                table: "PersonasDocumentos",
                column: "NumeroDocumento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonasTelefonos_IdCodigoTelefono",
                table: "PersonasTelefonos",
                column: "IdCodigoTelefono");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasTelefonos_IdPersona_IdCodigoTelefono_NumeroTelefono",
                table: "PersonasTelefonos",
                columns: new[] { "IdPersona", "IdCodigoTelefono", "NumeroTelefono" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_IdCategoriaRepuesto",
                table: "Repuestos",
                column: "IdCategoriaRepuesto");

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_IdTaller_Codigo",
                table: "Repuestos",
                columns: new[] { "IdTaller", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talleres_Nit",
                table: "Talleres",
                column: "Nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposAccionAuditoria_Nombre",
                table: "TiposAccionAuditoria",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposDocumentos_Codigo",
                table: "TiposDocumentos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposServicio_Nombre",
                table: "TiposServicio",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdPersona",
                table: "Usuarios",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdTaller_IdPersona",
                table: "Usuarios",
                columns: new[] { "IdTaller", "IdPersona" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_IdRol",
                table: "UsuariosRoles",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_IdColor",
                table: "Vehiculos",
                column: "IdColor");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_IdModelo",
                table: "Vehiculos",
                column: "IdModelo");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_IdTaller_Placa",
                table: "Vehiculos",
                columns: new[] { "IdTaller", "Placa" },
                unique: true,
                filter: "\"Placa\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_IdTaller_VIN",
                table: "Vehiculos",
                columns: new[] { "IdTaller", "VIN" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "DetallesFactura");

            migrationBuilder.DropTable(
                name: "HistorialDuenosVehiculo");

            migrationBuilder.DropTable(
                name: "OrdenesServicio_Repuestos");

            migrationBuilder.DropTable(
                name: "PersonasCorreos");

            migrationBuilder.DropTable(
                name: "PersonasDocumentos");

            migrationBuilder.DropTable(
                name: "PersonasTelefonos");

            migrationBuilder.DropTable(
                name: "UsuariosRoles");

            migrationBuilder.DropTable(
                name: "TiposAccionAuditoria");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Repuestos");

            migrationBuilder.DropTable(
                name: "DominiosCorreo");

            migrationBuilder.DropTable(
                name: "TiposDocumentos");

            migrationBuilder.DropTable(
                name: "CodigosTelefono");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "MetodosPago");

            migrationBuilder.DropTable(
                name: "OrdenesServicio");

            migrationBuilder.DropTable(
                name: "CategoriasRepuesto");

            migrationBuilder.DropTable(
                name: "EstadosOrden");

            migrationBuilder.DropTable(
                name: "TiposServicio");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Vehiculos");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "ColoresVehiculo");

            migrationBuilder.DropTable(
                name: "ModelosVehiculo");

            migrationBuilder.DropTable(
                name: "Talleres");

            migrationBuilder.DropTable(
                name: "MarcasVehiculo");
        }
    }
}
