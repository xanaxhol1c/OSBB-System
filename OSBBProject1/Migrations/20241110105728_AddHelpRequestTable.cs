using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSBBProject1.Migrations
{
    /// <inheritdoc />
    public partial class AddHelpRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlatNumber",
                table: "HelpRequests");

            migrationBuilder.RenameColumn(
                name: "RequestDate",
                table: "HelpRequests",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "HelpRequests",
                newName: "Text");

            migrationBuilder.AddColumn<DateTime>(
                name: "SendDate",
                table: "HelpRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendDate",
                table: "HelpRequests");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "HelpRequests",
                newName: "RequestDate");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "HelpRequests",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "FlatNumber",
                table: "HelpRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
