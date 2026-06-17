using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseCategoryId = table.Column<int>(type: "int", nullable: false),
                    LicenseExpirationDate = table.Column<DateTime>(type: "date", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    SupervisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TravelRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestingArea = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PassengerCount = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DepartureDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TripPurpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancelledByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedVehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedDriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VehicleTypeId = table.Column<int>(type: "int", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Mileage = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMaintenanceDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActualDepartureTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualReturnTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuelRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "date", nullable: false),
                    Gallons = table.Column<decimal>(type: "decimal(8,3)", nullable: false),
                    PricePerGallon = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MileageAtRefuel = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelRecords_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceTypeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "date", nullable: false),
                    EstimatedExitDate = table.Column<DateTime>(type: "date", nullable: true),
                    ActualExitDate = table.Column<DateTime>(type: "date", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Workshop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NextMaintenanceDateScheduled = table.Column<DateTime>(type: "date", nullable: true),
                    NextMaintenanceKmScheduled = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceRecords_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ScheduleId",
                table: "Assignments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_StatusId",
                table: "Assignments",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "UQ_Assignments_RequestId",
                table: "Assignments",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_LicenseExpiry",
                table: "Drivers",
                column: "LicenseExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_StatusId",
                table: "Drivers",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_SupervisorId",
                table: "Drivers",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "UQ_Drivers_IdNumber",
                table: "Drivers",
                column: "IdNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Drivers_LicenseNumber",
                table: "Drivers",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelRecords_Date",
                table: "FuelRecords",
                column: "RecordDate");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRecords_Vehicle_Date",
                table: "FuelRecords",
                columns: new[] { "VehicleId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FuelRecords_VehicleId",
                table: "FuelRecords",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenance_EntryDate",
                table: "MaintenanceRecords",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenance_NextDate",
                table: "MaintenanceRecords",
                column: "NextMaintenanceDateScheduled");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenance_VehicleId",
                table: "MaintenanceRecords",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "UQ_Schedules_PeriodDate",
                table: "Schedules",
                column: "PeriodDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelRequests_ApprovedBy",
                table: "TravelRequests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelRequests_RequestedBy",
                table: "TravelRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelRequests_StatusId",
                table: "TravelRequests",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "UQ_TravelRequests_Number",
                table: "TravelRequests",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_StatusId",
                table: "Vehicles",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "UQ_Vehicles_LicensePlate",
                table: "Vehicles",
                column: "LicensePlate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "FuelRecords");

            migrationBuilder.DropTable(
                name: "MaintenanceRecords");

            migrationBuilder.DropTable(
                name: "TravelRequests");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
