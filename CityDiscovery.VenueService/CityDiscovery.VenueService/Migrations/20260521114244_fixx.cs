using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityDiscovery.VenueService.Migrations
{
    /// <inheritdoc />
    public partial class fixx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Venues_VenueAddresses_AddressId",
                table: "Venues");

            migrationBuilder.DropIndex(
                name: "IX_Venues_AddressId",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Venues");

            migrationBuilder.CreateIndex(
                name: "IX_VenueAddresses_CityId",
                table: "VenueAddresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueAddresses_CountryId",
                table: "VenueAddresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueAddresses_DistrictId",
                table: "VenueAddresses",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueAddresses_VenueId",
                table: "VenueAddresses",
                column: "VenueId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VenueAddresses_Cities_CityId",
                table: "VenueAddresses",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VenueAddresses_Countries_CountryId",
                table: "VenueAddresses",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VenueAddresses_Districts_DistrictId",
                table: "VenueAddresses",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VenueAddresses_Venues_VenueId",
                table: "VenueAddresses",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VenueAddresses_Cities_CityId",
                table: "VenueAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_VenueAddresses_Countries_CountryId",
                table: "VenueAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_VenueAddresses_Districts_DistrictId",
                table: "VenueAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_VenueAddresses_Venues_VenueId",
                table: "VenueAddresses");

            migrationBuilder.DropIndex(
                name: "IX_VenueAddresses_CityId",
                table: "VenueAddresses");

            migrationBuilder.DropIndex(
                name: "IX_VenueAddresses_CountryId",
                table: "VenueAddresses");

            migrationBuilder.DropIndex(
                name: "IX_VenueAddresses_DistrictId",
                table: "VenueAddresses");

            migrationBuilder.DropIndex(
                name: "IX_VenueAddresses_VenueId",
                table: "VenueAddresses");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Venues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venues_AddressId",
                table: "Venues",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Venues_VenueAddresses_AddressId",
                table: "Venues",
                column: "AddressId",
                principalTable: "VenueAddresses",
                principalColumn: "Id");
        }
    }
}
