﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS1_2024_25.API.Migrations
{
    /// <inheritdoc />
    public partial class YOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YearOfStudies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GodinaStudija = table.Column<int>(type: "int", nullable: false),
                    DatumUpisa = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AkademskaGodinaId = table.Column<int>(type: "int", nullable: false),
                    CijenaSkolarine = table.Column<float>(type: "real", nullable: false),
                    Obnova = table.Column<bool>(type: "bit", nullable: false),
                    DatumOvjere = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Napomena = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnimioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearOfStudies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearOfStudies_AcademicYears_AkademskaGodinaId",
                        column: x => x.AkademskaGodinaId,
                        principalTable: "AcademicYears",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_YearOfStudies_MyAppUsers_SnimioId",
                        column: x => x.SnimioId,
                        principalTable: "MyAppUsers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_YearOfStudies_AkademskaGodinaId",
                table: "YearOfStudies",
                column: "AkademskaGodinaId");

            migrationBuilder.CreateIndex(
                name: "IX_YearOfStudies_SnimioId",
                table: "YearOfStudies",
                column: "SnimioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YearOfStudies");
        }
    }
}
