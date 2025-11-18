using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinal.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialAcademico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatosExtra = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoMatricula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Terminos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creditos = table.Column<int>(type: "int", nullable: false),
                    TerminoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCreadorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificadorId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cursos_Terminos_TerminoId",
                        column: x => x.TerminoId,
                        principalTable: "Terminos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocenteCursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    Horario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocenteCursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocenteCursos_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    TerminoId = table.Column<int>(type: "int", nullable: false),
                    NotaFinal = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Terminos_TerminoId",
                        column: x => x.TerminoId,
                        principalTable: "Terminos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_TerminoId",
                table: "Cursos",
                column: "TerminoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocenteCursos_CursoId",
                table: "DocenteCursos",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_CodigoMatricula",
                table: "Estudiantes",
                column: "CodigoMatricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_Correo",
                table: "Estudiantes",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_Identificacion",
                table: "Estudiantes",
                column: "Identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_CursoId",
                table: "Evaluaciones",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_EstudianteId",
                table: "Evaluaciones",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_TerminoId",
                table: "Evaluaciones",
                column: "TerminoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "DocenteCursos");

            migrationBuilder.DropTable(
                name: "Evaluaciones");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "Terminos");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
