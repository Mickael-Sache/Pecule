using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pecule.Imports.Migrations
{
    /// <inheritdoc />
    public partial class InitialImports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "imports");

            migrationBuilder.CreateTable(
                name: "Imports",
                schema: "imports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Format = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ImportedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LineCount = table.Column<int>(type: "integer", nullable: false),
                    ImportedCount = table.Column<int>(type: "integer", nullable: false),
                    DuplicateCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "imports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    RawLabel = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NormalizedLabel = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Fingerprint = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Imports_ImportId",
                        column: x => x.ImportId,
                        principalSchema: "imports",
                        principalTable: "Imports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Imports_UserId_ImportedAt",
                schema: "imports",
                table: "Imports",
                columns: new[] { "UserId", "ImportedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId_Date",
                schema: "imports",
                table: "Transactions",
                columns: new[] { "AccountId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId_Fingerprint",
                schema: "imports",
                table: "Transactions",
                columns: new[] { "AccountId", "Fingerprint" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ImportId",
                schema: "imports",
                table: "Transactions",
                column: "ImportId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                schema: "imports",
                table: "Transactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "imports");

            migrationBuilder.DropTable(
                name: "Imports",
                schema: "imports");
        }
    }
}
