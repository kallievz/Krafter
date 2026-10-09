using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AditiKraft.Krafter.Backend.Migrations.Localization;

/// <inheritdoc />
public partial class InitLocalization : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "Ui");

        migrationBuilder.CreateTable(
            name: "GridState",
            schema: "Ui",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                ComponentKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                StateJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsDefault = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GridState", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Language",
            schema: "Ui",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Language", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Resource",
            schema: "Ui",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Key = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Resource", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ResourceTranslation",
            schema: "Ui",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                ResourceId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                LanguageId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResourceTranslation", x => x.Id);
                table.ForeignKey(
                    name: "FK_ResourceTranslation_Language_LanguageId",
                    column: x => x.LanguageId,
                    principalSchema: "Ui",
                    principalTable: "Language",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ResourceTranslation_Resource_ResourceId",
                    column: x => x.ResourceId,
                    principalSchema: "Ui",
                    principalTable: "Resource",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_GridState_TenantId_UserId_ComponentKey_IsDefault",
            schema: "Ui",
            table: "GridState",
            columns: new[] { "TenantId", "UserId", "ComponentKey", "IsDefault" });

        migrationBuilder.CreateIndex(
            name: "IX_GridState_TenantId_UserId_ComponentKey_Name",
            schema: "Ui",
            table: "GridState",
            columns: new[] { "TenantId", "UserId", "ComponentKey", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Language_Code",
            schema: "Ui",
            table: "Language",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Resource_Key",
            schema: "Ui",
            table: "Resource",
            column: "Key",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ResourceTranslation_LanguageId",
            schema: "Ui",
            table: "ResourceTranslation",
            column: "LanguageId");

        migrationBuilder.CreateIndex(
            name: "IX_ResourceTranslation_ResourceId_LanguageId",
            schema: "Ui",
            table: "ResourceTranslation",
            columns: new[] { "ResourceId", "LanguageId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GridState",
            schema: "Ui");

        migrationBuilder.DropTable(
            name: "ResourceTranslation",
            schema: "Ui");

        migrationBuilder.DropTable(
            name: "Language",
            schema: "Ui");

        migrationBuilder.DropTable(
            name: "Resource",
            schema: "Ui");
    }
}
