using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AditiKraft.Krafter.Backend.Migrations.ApplicationDb;

/// <inheritdoc />
public partial class AddMunsoftInstanceAndIntegrations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "integration");

        migrationBuilder.EnsureSchema(
            name: "munsoft");

        migrationBuilder.CreateTable(
            name: "ExternalTokenStorage",
            schema: "integration",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                ExpiresOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExternalTokenStorage", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "MunsoftInstance",
            schema: "munsoft",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                MunicipalityName = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                DemarcationCode = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MunsoftInstance", x => x.Id);
                table.ForeignKey(
                    name: "FK_MunsoftInstance_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_MunsoftInstance_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ExternalTokenStorage_Key",
            schema: "integration",
            table: "ExternalTokenStorage",
            column: "Key",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_MunsoftInstance_CreatedById",
            schema: "munsoft",
            table: "MunsoftInstance",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_MunsoftInstance_DemarcationCode",
            schema: "munsoft",
            table: "MunsoftInstance",
            column: "DemarcationCode");

        migrationBuilder.CreateIndex(
            name: "IX_MunsoftInstance_UpdatedById",
            schema: "munsoft",
            table: "MunsoftInstance",
            column: "UpdatedById");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ExternalTokenStorage",
            schema: "integration");

        migrationBuilder.DropTable(
            name: "MunsoftInstance",
            schema: "munsoft");
    }
}
