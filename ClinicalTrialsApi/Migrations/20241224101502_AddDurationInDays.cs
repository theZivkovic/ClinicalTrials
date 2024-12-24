﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicalTrialsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationInDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "ClinicalTrialMetadatas",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "ClinicalTrialMetadatas");
        }
    }
}
