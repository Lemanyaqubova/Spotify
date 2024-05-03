using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Migrations
{
    public partial class ratingColumndelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Ratings_RatingId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RatingId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RatingId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatingId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RatingId",
                table: "AspNetUsers",
                column: "RatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Ratings_RatingId",
                table: "AspNetUsers",
                column: "RatingId",
                principalTable: "Ratings",
                principalColumn: "Id");
        }
    }
}
