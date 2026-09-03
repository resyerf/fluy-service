using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fluy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequiredRoleId",
                schema: "tenant",
                table: "Approvals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tier",
                schema: "tenant",
                table: "Approvals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ApprovalRules",
                schema: "tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SecondApproverRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRules_TenantId",
                schema: "tenant",
                table: "ApprovalRules",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRules",
                schema: "tenant");

            migrationBuilder.DropColumn(
                name: "RequiredRoleId",
                schema: "tenant",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "Tier",
                schema: "tenant",
                table: "Approvals");
        }
    }
}
