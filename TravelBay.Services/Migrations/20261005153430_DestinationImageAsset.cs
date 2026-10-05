using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBay.Services.Migrations
{
    /// <inheritdoc />
    public partial class DestinationImageAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "DestinationImages",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 2,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 3,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 4,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 5,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 6,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 7,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 8,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 9,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 10,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 11,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 12,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 13,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 14,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 15,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 16,
                column: "AssetId",
                value: null);

            migrationBuilder.UpdateData(
                table: "DestinationImages",
                keyColumn: "Id",
                keyValue: 17,
                column: "AssetId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_DestinationImages_AssetId",
                table: "DestinationImages",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_DestinationImages_Assets_AssetId",
                table: "DestinationImages",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DestinationImages_Assets_AssetId",
                table: "DestinationImages");

            migrationBuilder.DropIndex(
                name: "IX_DestinationImages_AssetId",
                table: "DestinationImages");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "DestinationImages");
        }
    }
}
