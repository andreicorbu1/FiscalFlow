using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscalFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Removeredundantcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}
