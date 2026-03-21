using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorPatientApp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceIds2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Users",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "TimeSlots",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Prescriptions",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Patients",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "MedicalRecords",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Doctors",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Appointments",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Admins",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReferenceId",
                table: "Users",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_ReferenceId",
                table: "TimeSlots",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_ReferenceId",
                table: "Prescriptions",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ReferenceId",
                table: "Patients",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_ReferenceId",
                table: "MedicalRecords",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ReferenceId",
                table: "Doctors",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ReferenceId",
                table: "Appointments",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_ReferenceId",
                table: "Admins",
                column: "ReferenceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_ReferenceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_ReferenceId",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_ReferenceId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ReferenceId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_ReferenceId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ReferenceId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ReferenceId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Admins_ReferenceId",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Admins");
        }
    }
}
