using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaveTheKitty.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTemporaryShelterToCat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Breed",
                table: "Cats");

            migrationBuilder.AddColumn<bool>(
                name: "HasTemporaryShelter",
                table: "Cats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HasTemporaryShelter",
                table: "Cats");

            migrationBuilder.AddColumn<string>(
                name: "Breed",
                table: "Cats",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
