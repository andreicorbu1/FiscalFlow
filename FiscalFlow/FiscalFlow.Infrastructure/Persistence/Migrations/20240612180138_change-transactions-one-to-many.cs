using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscalFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changetransactionsonetomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecursiveTransactions_Transactions_TransactionId",
                table: "RecursiveTransactions");

            migrationBuilder.DropIndex(
                name: "IX_RecursiveTransactions_TransactionId",
                table: "RecursiveTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "RecursiveTransactions");

            migrationBuilder.AddColumn<Guid>(
                name: "RecursiveTransactionId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RecursiveTransactionId",
                table: "Transactions",
                column: "RecursiveTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_RecursiveTransactions_RecursiveTransactionId",
                table: "Transactions",
                column: "RecursiveTransactionId",
                principalTable: "RecursiveTransactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_RecursiveTransactions_RecursiveTransactionId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_RecursiveTransactionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RecursiveTransactionId",
                table: "Transactions");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "RecursiveTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RecursiveTransactions_TransactionId",
                table: "RecursiveTransactions",
                column: "TransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecursiveTransactions_Transactions_TransactionId",
                table: "RecursiveTransactions",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
