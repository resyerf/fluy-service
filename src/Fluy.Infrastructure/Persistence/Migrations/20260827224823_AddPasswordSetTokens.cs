using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fluy.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordSetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordSetTokens",
                schema: "tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordSetTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordSetTokens_TenantId_UserId",
                schema: "tenant",
                table: "PasswordSetTokens",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordSetTokens_TokenHash",
                schema: "tenant",
                table: "PasswordSetTokens",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordSetTokens",
                schema: "tenant");
        }
    }
}
