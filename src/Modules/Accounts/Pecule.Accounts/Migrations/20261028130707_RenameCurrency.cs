using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pecule.Accounts.Migrations
{
    /// <inheritdoc />
    public partial class RenameCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Devise",
                schema: "accounts",
                table: "Accounts",
                newName: "Currency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Currency",
                schema: "accounts",
                table: "Accounts",
                newName: "Devise");
        }
    }
}
