using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VulnerableApp.Migrations
{
    /// <inheritdoc />
    public partial class InicialLimpia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Balance", "CreatedAt", "Email", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1, 1000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@test.com", "$2b$12$4ViAVOygLkUqH7zS88Va8eqnYT8SNNSLUWsUEtDiqtpAVY0anO9Aq", "admin" },
                    { 2, 500m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user@test.com", "$2b$12$8kUykxelX5lI8iQ/DP1oTeuK/1sWesogHN.TVZR.hETNNDUslBVTW", "user1" },
                    { 3, 750m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user2@test.com", "$$2b$12$fDaWV7Cx6ON8GPx4Pdb8Q.tzCVtu/xxl4j7RHQF20HLviyeGL/V26", "user2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
