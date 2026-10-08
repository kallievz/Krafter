using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AditiKraft.Krafter.Backend.Migrations;

public partial class UpdateAuditOutboxAddFailure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "FailureCount",
            table: "AuditOutbox",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "LastError",
            table: "AuditOutbox",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_AuditOutbox_ProcessedOn",
            table: "AuditOutbox",
            column: "ProcessedOn");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AuditOutbox_ProcessedOn",
            table: "AuditOutbox");

        migrationBuilder.DropColumn(
            name: "FailureCount",
            table: "AuditOutbox");

        migrationBuilder.DropColumn(
            name: "LastError",
            table: "AuditOutbox");
    }
}
