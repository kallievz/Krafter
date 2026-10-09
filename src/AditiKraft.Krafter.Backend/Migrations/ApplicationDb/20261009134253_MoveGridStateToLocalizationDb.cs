using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AditiKraft.Krafter.Backend.Migrations.ApplicationDb;

/// <inheritdoc />
public partial class MoveGridStateToLocalizationDb : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GridState",
            schema: "app");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "app");

        migrationBuilder.CreateTable(
            name: "GridState",
            schema: "app",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                ComponentKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                IsDefault = table.Column<bool>(type: "bit", nullable: false),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                StateJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GridState", x => x.Id);
                table.ForeignKey(
                    name: "FK_GridState_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_GridState_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_GridState_CreatedById",
            schema: "app",
            table: "GridState",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_GridState_TenantId_UserId_ComponentKey_IsDefault",
            schema: "app",
            table: "GridState",
            columns: new[] { "TenantId", "UserId", "ComponentKey", "IsDefault" });

        migrationBuilder.CreateIndex(
            name: "IX_GridState_TenantId_UserId_ComponentKey_Name",
            schema: "app",
            table: "GridState",
            columns: new[] { "TenantId", "UserId", "ComponentKey", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_GridState_UpdatedById",
            schema: "app",
            table: "GridState",
            column: "UpdatedById");
    }
}
