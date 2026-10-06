using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelBay.Services.Migrations
{
    /// <inheritdoc />
    public partial class NewsPublishedAtAndImageAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageAssetId",
                table: "News",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                table: "News",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageAssetId", "PublishedAt" },
                values: new object[] { null, new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageAssetId", "PublishedAt" },
                values: new object[] { null, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageAssetId", "PublishedAt" },
                values: new object[] { null, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImageAssetId", "PublishedAt" },
                values: new object[] { null, new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ImageAssetId", "PublishedAt" },
                values: new object[] { null, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc) });

            // Rows created before this column existed are published when they were created.
            migrationBuilder.Sql("UPDATE [News] SET [PublishedAt] = [CreatedAt] WHERE [PublishedAt] = '0001-01-01'");

            migrationBuilder.CreateIndex(
                name: "IX_News_ImageAssetId",
                table: "News",
                column: "ImageAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Assets_ImageAssetId",
                table: "News",
                column: "ImageAssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Assets_ImageAssetId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_ImageAssetId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ImageAssetId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "News");
        }
    }
}
