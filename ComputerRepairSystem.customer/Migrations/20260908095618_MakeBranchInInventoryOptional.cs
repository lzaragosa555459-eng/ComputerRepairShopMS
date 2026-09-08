using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerRepairSystem.company.Migrations
{
    /// <inheritdoc />
    public partial class MakeBranchInInventoryOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventories_BranchId_ItemId",
                table: "Inventories");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "Inventories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_BranchId_ItemId",
                table: "Inventories",
                columns: new[] { "BranchId", "ItemId" },
                unique: true,
                filter: "[BranchId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventories_BranchId_ItemId",
                table: "Inventories");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_BranchId_ItemId",
                table: "Inventories",
                columns: new[] { "BranchId", "ItemId" },
                unique: true);
        }
    }
}
