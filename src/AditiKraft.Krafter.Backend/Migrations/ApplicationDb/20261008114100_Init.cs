using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AditiKraft.Krafter.Backend.Migrations.ApplicationDb;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "identity");

        migrationBuilder.EnsureSchema(
            name: "comms");

        migrationBuilder.CreateTable(
            name: "AppUser",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", nullable: true),
                LastPasswordChangedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastPasswordExpiryNotificationOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                IsOwner = table.Column<bool>(type: "bit", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppUser", x => x.Id);
                table.ForeignKey(
                    name: "FK_AppUser_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AppUser_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "UserRefreshTokens",
            schema: "identity",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRefreshTokens", x => x.UserId);
            });

        migrationBuilder.CreateTable(
            name: "AppRole",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppRole", x => x.Id);
                table.ForeignKey(
                    name: "FK_AppRole_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AppRole_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AppUserClaim",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", maxLength: 36, nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppUserClaim", x => x.Id);
                table.ForeignKey(
                    name: "FK_AppUserClaim_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AppUserClaim_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AppUserClaim_AppUser_UserId",
                    column: x => x.UserId,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AppUserLogin",
            schema: "identity",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<string>(type: "nvarchar(36)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppUserLogin", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AppUserLogin_AppUser_UserId",
                    column: x => x.UserId,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AppUserToken",
            schema: "identity",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppUserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AppUserToken_AppUser_UserId",
                    column: x => x.UserId,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "EmailTemplates",
            schema: "comms",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Key = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                SubjectTemplate = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", nullable: true),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                table.ForeignKey(
                    name: "FK_EmailTemplates_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EmailTemplates_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "PasswordHistories",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", nullable: true),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PasswordHistories", x => x.Id);
                table.ForeignKey(
                    name: "FK_PasswordHistories_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PasswordHistories_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PasswordHistories_AppUser_UserId",
                    column: x => x.UserId,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AppRoleClaim",
            schema: "identity",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", maxLength: 36, nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppRoleClaim", x => x.Id);
                table.ForeignKey(
                    name: "FK_AppRoleClaim_AppRole_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "identity",
                    principalTable: "AppRole",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AppRoleClaim_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AppRoleClaim_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AppUserRole",
            schema: "identity",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                RoleId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppUserRole", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AppUserRole_AppRole_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "identity",
                    principalTable: "AppRole",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AppUserRole_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AppUserRole_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AppUserRole_AppUser_UserId",
                    column: x => x.UserId,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "EmailLogs",
            schema: "comms",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                ToEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                SentOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                EmailTemplateId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                DeleteReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                CreatedById = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedById = table.Column<string>(type: "nvarchar(36)", nullable: true),
                TenantId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmailLogs", x => x.Id);
                table.ForeignKey(
                    name: "FK_EmailLogs_AppUser_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EmailLogs_AppUser_UpdatedById",
                    column: x => x.UpdatedById,
                    principalSchema: "identity",
                    principalTable: "AppUser",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EmailLogs_EmailTemplates_EmailTemplateId",
                    column: x => x.EmailTemplateId,
                    principalSchema: "comms",
                    principalTable: "EmailTemplates",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AppRole_CreatedById",
            schema: "identity",
            table: "AppRole",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_AppRole_NormalizedName_TenantId",
            schema: "identity",
            table: "AppRole",
            columns: new[] { "NormalizedName", "TenantId" },
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AppRole_UpdatedById",
            schema: "identity",
            table: "AppRole",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            schema: "identity",
            table: "AppRole",
            column: "NormalizedName",
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AppRoleClaim_CreatedById",
            schema: "identity",
            table: "AppRoleClaim",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_AppRoleClaim_RoleId",
            schema: "identity",
            table: "AppRoleClaim",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_AppRoleClaim_UpdatedById",
            schema: "identity",
            table: "AppRoleClaim",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            schema: "identity",
            table: "AppUser",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "IX_AppUser_CreatedById",
            schema: "identity",
            table: "AppUser",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_AppUser_NormalizedEmail_TenantId",
            schema: "identity",
            table: "AppUser",
            columns: new[] { "NormalizedEmail", "TenantId" },
            unique: true,
            filter: "[NormalizedEmail] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AppUser_NormalizedUserName_TenantId",
            schema: "identity",
            table: "AppUser",
            columns: new[] { "NormalizedUserName", "TenantId" },
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AppUser_UpdatedById",
            schema: "identity",
            table: "AppUser",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            schema: "identity",
            table: "AppUser",
            column: "NormalizedUserName");

        migrationBuilder.CreateIndex(
            name: "IX_AppUserClaim_CreatedById",
            schema: "identity",
            table: "AppUserClaim",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_AppUserClaim_UpdatedById",
            schema: "identity",
            table: "AppUserClaim",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "IX_AppUserClaim_UserId",
            schema: "identity",
            table: "AppUserClaim",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AppUserLogin_UserId",
            schema: "identity",
            table: "AppUserLogin",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AppUserRole_CreatedById",
            schema: "identity",
            table: "AppUserRole",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_AppUserRole_RoleId",
            schema: "identity",
            table: "AppUserRole",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_AppUserRole_UpdatedById",
            schema: "identity",
            table: "AppUserRole",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "IX_EmailLogs_CreatedById",
            schema: "comms",
            table: "EmailLogs",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_EmailLogs_EmailTemplateId",
            schema: "comms",
            table: "EmailLogs",
            column: "EmailTemplateId");

        migrationBuilder.CreateIndex(
            name: "IX_EmailLogs_TenantId_Status_CreatedOn",
            schema: "comms",
            table: "EmailLogs",
            columns: new[] { "TenantId", "Status", "CreatedOn" });

        migrationBuilder.CreateIndex(
            name: "IX_EmailLogs_UpdatedById",
            schema: "comms",
            table: "EmailLogs",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "IX_EmailTemplates_CreatedById",
            schema: "comms",
            table: "EmailTemplates",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_EmailTemplates_TenantId_Key",
            schema: "comms",
            table: "EmailTemplates",
            columns: new[] { "TenantId", "Key" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_EmailTemplates_UpdatedById",
            schema: "comms",
            table: "EmailTemplates",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "IX_PasswordHistories_CreatedById",
            schema: "identity",
            table: "PasswordHistories",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_PasswordHistories_UpdatedById",
            schema: "identity",
            table: "PasswordHistories",
            column: "UpdatedById");

        migrationBuilder.CreateIndex(
            name: "IX_PasswordHistories_UserId_CreatedOn",
            schema: "identity",
            table: "PasswordHistories",
            columns: new[] { "UserId", "CreatedOn" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AppRoleClaim",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "AppUserClaim",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "AppUserLogin",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "AppUserRole",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "AppUserToken",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "EmailLogs",
            schema: "comms");

        migrationBuilder.DropTable(
            name: "PasswordHistories",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "UserRefreshTokens",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "AppRole",
            schema: "identity");

        migrationBuilder.DropTable(
            name: "EmailTemplates",
            schema: "comms");

        migrationBuilder.DropTable(
            name: "AppUser",
            schema: "identity");
    }
}
