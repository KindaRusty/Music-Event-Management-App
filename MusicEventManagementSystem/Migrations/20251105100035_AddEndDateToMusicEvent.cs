using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicEventManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEndDateToMusicEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "MusicEvents",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "MusicEvents");
        }
    }
}
