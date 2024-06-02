using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscalFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "RecursiveTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecursiveTransactions_OwnerId",
                table: "RecursiveTransactions",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecursiveTransactions_AspNetUsers_OwnerId",
                table: "RecursiveTransactions",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecursiveTransactions_AspNetUsers_OwnerId",
                table: "RecursiveTransactions");

            migrationBuilder.DropIndex(
                name: "IX_RecursiveTransactions_OwnerId",
                table: "RecursiveTransactions");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "RecursiveTransactions");
        }
    }
}
