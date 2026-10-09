using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AditiKraft.Krafter.Backend.Migrations.ApplicationDb;

/// <inheritdoc />
public partial class RemoveMunsoftFromApplicationDb : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "MunsoftInstance",
            schema: "munsoft");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "munsoft");

        migrationBuilder.CreateTable(
            name: "MunsoftInstance",
            schema: "munsoft",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                DemarcationCode = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                MunicipalityName = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                Name = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
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
}
