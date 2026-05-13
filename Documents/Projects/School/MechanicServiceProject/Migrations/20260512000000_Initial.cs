using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace MechanicService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Make = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YearOfManufacture = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAppointments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceAppointments_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleServiceTypes",
                columns: table => new
                {
                    ServiceTypesId = table.Column<int>(type: "int", nullable: false),
                    VehiclesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServiceTypes", x => new { x.ServiceTypesId, x.VehiclesId });
                    table.ForeignKey(
                        name: "FK_VehicleServiceTypes_ServiceTypes_ServiceTypesId",
                        column: x => x.ServiceTypesId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleServiceTypes_Vehicles_VehiclesId",
                        column: x => x.VehiclesId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Seed data
            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "Name", "Description" },
                values: new object[,]
                {
                    { 1, "Смяна на масло", "Пълна смяна на двигателното масло и маслен филтър" },
                    { 2, "Смяна на спирачки", "Проверка и смяна на спирачни накладки и дискове" },
                    { 3, "Компютърна диагностика", "Пълна диагностика на електронните системи" },
                    { 4, "Смяна на гуми", "Смяна и балансиране на гуми" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "IsAvailable", "LicensePlate", "Make", "Model", "YearOfManufacture" },
                values: new object[,]
                {
                    { 1, true, "CB1234AB", "Toyota", "Corolla", 2018 },
                    { 2, true, "CB5678CD", "BMW", "320i", 2020 },
                    { 3, false, "CB9012EF", "Ford", "Focus", 2015 }
                });

            migrationBuilder.InsertData(
                table: "VehicleServiceTypes",
                columns: new[] { "ServiceTypesId", "VehiclesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 3, 3 },
                    { 4, 3 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "FullName", "Phone" },
                values: new object[,]
                {
                    { 1, "ivan@example.com", "Иван Петров", "+359888111222" },
                    { 2, "maria@example.com", "Мария Иванова", "+359888333444" }
                });

            migrationBuilder.InsertData(
                table: "ServiceAppointments",
                columns: new[] { "Id", "AppointmentDate", "CompletionDate", "CustomerId", "TotalPrice", "VehicleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 1, 14, 0, 0, DateTimeKind.Unspecified), 1, 120m, 1 },
                    { 2, new DateTime(2026, 7, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 7, 10, 17, 0, 0, DateTimeKind.Unspecified), 2, 350m, 2 },
                    { 3, new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 2, 12, 0, 0, DateTimeKind.Unspecified), 1, 200m, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAppointments_CustomerId",
                table: "ServiceAppointments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAppointments_VehicleId",
                table: "ServiceAppointments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceTypes_VehiclesId",
                table: "VehicleServiceTypes",
                column: "VehiclesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ServiceAppointments");
            migrationBuilder.DropTable(name: "VehicleServiceTypes");
            migrationBuilder.DropTable(name: "Customers");
            migrationBuilder.DropTable(name: "ServiceTypes");
            migrationBuilder.DropTable(name: "Vehicles");
        }
    }
}
