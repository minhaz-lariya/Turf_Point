using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Turf_Point.Migrations
{
    /// <inheritdoc />
    public partial class merry_christmass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "paymentMasters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_BookingSlots_TimeslotId",
                table: "BookingSlots",
                column: "TimeslotId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingMasters_RegistrationId",
                table: "BookingMasters",
                column: "RegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingMasters_registrations_RegistrationId",
                table: "BookingMasters",
                column: "RegistrationId",
                principalTable: "registrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSlots_timeslots_TimeslotId",
                table: "BookingSlots",
                column: "TimeslotId",
                principalTable: "timeslots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingMasters_registrations_RegistrationId",
                table: "BookingMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSlots_timeslots_TimeslotId",
                table: "BookingSlots");

            migrationBuilder.DropIndex(
                name: "IX_BookingSlots_TimeslotId",
                table: "BookingSlots");

            migrationBuilder.DropIndex(
                name: "IX_BookingMasters_RegistrationId",
                table: "BookingMasters");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "paymentMasters");
        }
    }
}
