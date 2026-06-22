using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledMaintenances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "date", nullable: false),
                    Workshop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ScheduledKm = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResultingMaintenanceRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMaintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledMaintenances_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMaintenance_ScheduledDate",
                table: "ScheduledMaintenances",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMaintenance_StatusId",
                table: "ScheduledMaintenances",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMaintenance_VehicleId",
                table: "ScheduledMaintenances",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledMaintenances");
        }
    }
}
