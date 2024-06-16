using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscalFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecursiveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecursiveTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Recurrence = table.Column<int>(type: "integer", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursiveTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecursiveTransactions_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecursiveTransactions_TransactionId",
                table: "RecursiveTransactions",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecursiveTransactions");
        }
    }
}
