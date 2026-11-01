using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KYC_Aggregation_Service.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregatedKycData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ssn = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    TaxCountry = table.Column<string>(type: "TEXT", nullable: true),
                    Income = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedKycData", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedKycData_Ssn",
                table: "AggregatedKycData",
                column: "Ssn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatedKycData");
        }
    }
}
