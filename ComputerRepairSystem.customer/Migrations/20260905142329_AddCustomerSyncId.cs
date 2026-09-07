using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerRepairSystem.company.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerSyncId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SyncId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_SyncId",
                table: "Customers",
                column: "SyncId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_SyncId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SyncId",
                table: "Customers");
        }
    }
}
