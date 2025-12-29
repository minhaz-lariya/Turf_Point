using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turf_Point.Migrations
{
    /// <inheritdoc />
    public partial class add_field_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BookingSlots",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookingSlots");
        }
    }
}
