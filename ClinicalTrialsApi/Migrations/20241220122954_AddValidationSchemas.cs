using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicalTrialsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddValidationSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ValidationSchemas",
                columns: table => new
                {
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Schema = table.Column<JsonElement>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationSchemas", x => x.Type);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValidationSchemas");
        }
    }
}
