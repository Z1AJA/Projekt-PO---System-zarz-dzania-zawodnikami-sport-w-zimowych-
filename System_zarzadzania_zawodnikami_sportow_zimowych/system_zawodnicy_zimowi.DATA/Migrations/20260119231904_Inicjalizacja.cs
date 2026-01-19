using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace system_zawodnicy_zimowi.DATA.Migrations
{
    /// <inheritdoc />
    public partial class Inicjalizacja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kluby",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimalnePunkty = table.Column<int>(type: "int", nullable: false),
                    MaksWiek = table.Column<int>(type: "int", nullable: true),
                    Dyscypliny = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LimitMiejsc = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kluby", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zawodnicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wiek = table.Column<int>(type: "int", nullable: false),
                    Dyscyplina = table.Column<int>(type: "int", nullable: false),
                    Punkty = table.Column<int>(type: "int", nullable: false),
                    Ranga = table.Column<int>(type: "int", nullable: false),
                    KlubId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KlubNazwa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypZawodnika = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zawodnicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wyniki",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NazwaZawodow = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Miejsce = table.Column<int>(type: "int", nullable: false),
                    TrudnoscTrasy = table.Column<int>(type: "int", nullable: false),
                    PunktyBazowe = table.Column<int>(type: "int", nullable: false),
                    ZawodnikId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wyniki", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wyniki_Zawodnicy_ZawodnikId",
                        column: x => x.ZawodnikId,
                        principalTable: "Zawodnicy",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wyniki_ZawodnikId",
                table: "Wyniki",
                column: "ZawodnikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kluby");

            migrationBuilder.DropTable(
                name: "Wyniki");

            migrationBuilder.DropTable(
                name: "Zawodnicy");
        }
    }
}
