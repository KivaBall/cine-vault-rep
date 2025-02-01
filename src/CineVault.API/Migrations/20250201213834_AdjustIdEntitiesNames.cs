using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineVault.API.Migrations
{
    /// <inheritdoc />
    public partial class AdjustIdEntitiesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reactions",
                table: "Reactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movies",
                table: "Movies");

            migrationBuilder.AddPrimaryKey(
                name: "UserId",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "ReviewId",
                table: "Reviews",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "ReactionId",
                table: "Reactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "MovieId",
                table: "Movies",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "UserId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "ReviewId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "ReactionId",
                table: "Reactions");

            migrationBuilder.DropPrimaryKey(
                name: "MovieId",
                table: "Movies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reactions",
                table: "Reactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movies",
                table: "Movies",
                column: "Id");
        }
    }
}
