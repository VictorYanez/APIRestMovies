using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace PeliculasAPIC.Migrations
{
    /// <inheritdoc />
    public partial class TablasSalasDeCines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PeliculasGeneros",
                table: "PeliculasGeneros");

            migrationBuilder.DropIndex(
                name: "IX_PeliculasGeneros_GeneroId",
                table: "PeliculasGeneros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PeliculasActores",
                table: "PeliculasActores");

            migrationBuilder.DropIndex(
                name: "IX_PeliculasActores_ActorId",
                table: "PeliculasActores");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PeliculasGeneros",
                table: "PeliculasGeneros",
                columns: new[] { "GeneroId", "PeliculaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PeliculasActores",
                table: "PeliculasActores",
                columns: new[] { "ActorId", "PeliculaId" });

            migrationBuilder.CreateTable(
                name: "SalasDeCine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Ubicacion = table.Column<Point>(type: "geography", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalasDeCine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PeliculasSalasDeCine",
                columns: table => new
                {
                    PeliculaId = table.Column<int>(type: "int", nullable: false),
                    SalaDeCineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeliculasSalasDeCine", x => new { x.PeliculaId, x.SalaDeCineId });
                    table.ForeignKey(
                        name: "FK_PeliculasSalasDeCine_Peliculas_PeliculaId",
                        column: x => x.PeliculaId,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeliculasSalasDeCine_SalasDeCine_SalaDeCineId",
                        column: x => x.SalaDeCineId,
                        principalTable: "SalasDeCine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasGeneros_PeliculaId",
                table: "PeliculasGeneros",
                column: "PeliculaId");

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasActores_PeliculaId",
                table: "PeliculasActores",
                column: "PeliculaId");

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasSalasDeCine_SalaDeCineId",
                table: "PeliculasSalasDeCine",
                column: "SalaDeCineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeliculasSalasDeCine");

            migrationBuilder.DropTable(
                name: "SalasDeCine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PeliculasGeneros",
                table: "PeliculasGeneros");

            migrationBuilder.DropIndex(
                name: "IX_PeliculasGeneros_PeliculaId",
                table: "PeliculasGeneros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PeliculasActores",
                table: "PeliculasActores");

            migrationBuilder.DropIndex(
                name: "IX_PeliculasActores_PeliculaId",
                table: "PeliculasActores");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PeliculasGeneros",
                table: "PeliculasGeneros",
                columns: new[] { "PeliculaId", "GeneroId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PeliculasActores",
                table: "PeliculasActores",
                columns: new[] { "PeliculaId", "ActorId" });

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasGeneros_GeneroId",
                table: "PeliculasGeneros",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasActores_ActorId",
                table: "PeliculasActores",
                column: "ActorId");
        }
    }
}
