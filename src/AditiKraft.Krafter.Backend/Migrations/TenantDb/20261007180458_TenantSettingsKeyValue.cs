using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AditiKraft.Krafter.Backend.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class TenantSettingsKeyValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantSetting",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    TenantId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    IsSensitive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSetting_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TenantSetting",
                columns: new[] { "Id", "IsSensitive", "Key", "TenantId", "Value" },
                values: new object[,]
                {
                    { "c8a78156-8e6c-4f55-8841-7de7ea101001", false, "Security:Password:ReusePreventionWindowInDays", "root", "90" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101002", false, "Security:Password:ExpiryInDays", "root", "90" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101003", false, "Security:Password:ExpiryNotificationCooldownInDays", "root", "1" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101004", false, "Notifications:Provider", "root", "smtp" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101005", false, "Notifications:Smtp:Host", "root", "" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101006", false, "Notifications:Smtp:Port", "root", "587" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101007", false, "Notifications:Smtp:UserName", "root", "" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101008", true, "Notifications:Smtp:Password", "root", "" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101009", false, "Notifications:Smtp:SenderEmail", "root", "" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101010", false, "Notifications:Smtp:SenderName", "root", "" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101011", true, "Notifications:SendGrid:ApiKey", "root", "" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101012", false, "Notifications:SendGrid:SenderEmail", "root", "" },
                    { "c8a78156-8e6c-4f55-8841-7de7ea101013", false, "Notifications:SendGrid:SenderName", "root", "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantSetting_TenantId_Key",
                table: "TenantSetting",
                columns: new[] { "TenantId", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantSetting");
        }
    }
}
