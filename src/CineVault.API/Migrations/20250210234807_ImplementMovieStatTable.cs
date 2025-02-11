using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineVault.API.Migrations
{
    /// <inheritdoc />
    public partial class ImplementMovieStatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieStats",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    MovieWasDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieStats", x => x.MovieId);
                    table.ForeignKey(
                        name: "FK_MovieStats_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieStats");
        }
    }
}
