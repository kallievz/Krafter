using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AditiKraft.Krafter.Backend.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class ReplyToTenantSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TenantSetting",
                keyColumn: "Id",
                keyValue: "c8a78156-8e6c-4f55-8841-7de7ea101011",
                columns: new[] { "IsSensitive", "Key" },
                values: new object[] { false, "Notifications:Smtp:ReplyToEmail" });

            migrationBuilder.UpdateData(
                table: "TenantSetting",
                keyColumn: "Id",
                keyValue: "c8a78156-8e6c-4f55-8841-7de7ea101012",
                columns: new[] { "IsSensitive", "Key" },
                values: new object[] { true, "Notifications:SendGrid:ApiKey" });

            migrationBuilder.UpdateData(
                table: "TenantSetting",
                keyColumn: "Id",
                keyValue: "c8a78156-8e6c-4f55-8841-7de7ea101013",
                column: "Key",
                value: "Notifications:SendGrid:SenderEmail");

            migrationBuilder.InsertData(
                table: "TenantSetting",
                columns: new[] { "Id", "IsSensitive", "Key", "TenantId", "Value" },
                values: new object[,]
                {
                    { "c8a78156-8e6c-4f55-8841-7de7ea101014", false, "Notifications:SendGrid:SenderName", "root", "" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101015", false, "Notifications:SendGrid:ReplyToEmail", "root", "" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TenantSetting",
                keyColumn: "Id",
                keyValue: "c8a78156-8e6c-4f55-8841-7de7ea101014");

            migrationBuilder.DeleteData(
                table: "TenantSetting",
                keyColumn: "Id",
                keyValue: "c8a78156-8e6c-4f55-8841-7de7ea101015");

            migrationBuilder.UpdateData(
                table: "TenantSetting",
                keyColumn: "Id",
                keyValue: "c8a78156-8e6c-4f55-8841-7de7ea101011",
                columns: new[] { "IsSensitive", "Key" },
                values: new object[] { true, "Notifications:SendGrid:ApiKey" });

            migrationBuilder.UpdateData(
                table: "TenantSetting",
                keyColumn: "Id",
                keyValue: "c8a78156-8e6c-4f55-8841-7de7ea101012",
                columns: new[] { "IsSensitive", "Key" },
                values: new object[] { false, "Notifications:SendGrid:SenderEmail" });

            migrationBuilder.UpdateData(
                table: "TenantSetting",
                keyColumn: "Id",
                keyValue: "c8a78156-8e6c-4f55-8841-7de7ea101013",
                column: "Key",
                value: "Notifications:SendGrid:SenderName");
        }
    }
}
