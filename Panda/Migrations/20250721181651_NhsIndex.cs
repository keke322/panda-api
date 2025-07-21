using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Panda.Migrations
{
    /// <inheritdoc />
    public partial class NhsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Patients_NhsNumber",
                table: "Patients",
                column: "NhsNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_NhsNumber",
                table: "Patients");
        }
    }
}
