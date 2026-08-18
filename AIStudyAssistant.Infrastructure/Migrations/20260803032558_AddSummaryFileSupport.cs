using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIStudyAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSummaryFileSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Summaries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Summaries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsGenerated",
                table: "Summaries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Summaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SummaryStyle",
                table: "Summaries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "IsGenerated",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "SummaryStyle",
                table: "Summaries");
        }
    }
}
