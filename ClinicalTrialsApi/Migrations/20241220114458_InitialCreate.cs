using System;
using ClinicalTrialsApi.Core.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicalTrialsApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:clinical_trials_status", "completed,not_started,ongoing");

            migrationBuilder.CreateTable(
                name: "ClinicalTrialMetadatas",
                columns: table => new
                {
                    TrialId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Participants = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<ClinicalTrialStatus>(type: "clinical_trials_status", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalTrialMetadatas", x => x.TrialId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicalTrialMetadatas");
        }
    }
}
