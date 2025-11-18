using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEstudiante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Estudiantes");
        }
    }
}
