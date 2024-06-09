using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscalFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRTTConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecursiveTransactions_TransactionId",
                table: "RecursiveTransactions");

            migrationBuilder.AddColumn<Guid>(
                name: "ReccursiveTransactionId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecursiveTransactions_TransactionId",
                table: "RecursiveTransactions",
                column: "TransactionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecursiveTransactions_TransactionId",
                table: "RecursiveTransactions");

            migrationBuilder.DropColumn(
                name: "ReccursiveTransactionId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_RecursiveTransactions_TransactionId",
                table: "RecursiveTransactions",
                column: "TransactionId");
        }
    }
}
