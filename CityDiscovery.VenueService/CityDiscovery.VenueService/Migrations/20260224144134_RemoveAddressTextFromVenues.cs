using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityDiscovery.VenueService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddressTextFromVenues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressText",
                table: "Venues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressText",
                table: "Venues",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);
        }
    }
}
