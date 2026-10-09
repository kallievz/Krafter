
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AditiKraft.Krafter.Backend.Migrations.Munsoft;

/// <inheritdoc />
public partial class InitMunsoft : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "munsoft");

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
            });

        migrationBuilder.CreateIndex(
            name: "IX_MunsoftInstance_DemarcationCode",
            schema: "munsoft",
            table: "MunsoftInstance",
            column: "DemarcationCode");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "MunsoftInstance",
            schema: "munsoft");
    }
}
