using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AditiKraft.Krafter.Backend.Migrations
{
    /// <inheritdoc />
    public partial class PasswordPolicyEnforcement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChangedOn",
                table: "ApplicationUser",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordExpiryNotificationOn",
                table: "ApplicationUser",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PasswordHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    DeleteReason = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedById = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<string>(type: "character varying(36)", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordHistory_ApplicationUser_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PasswordHistory_ApplicationUser_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PasswordHistory_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistory_CreatedById",
                table: "PasswordHistory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistory_UpdatedById",
                table: "PasswordHistory",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistory_UserId_CreatedOn",
                table: "PasswordHistory",
                columns: new[] { "UserId", "CreatedOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordHistory");

            migrationBuilder.DropColumn(
                name: "LastPasswordChangedOn",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LastPasswordExpiryNotificationOn",
                table: "ApplicationUser");
        }
    }
}
