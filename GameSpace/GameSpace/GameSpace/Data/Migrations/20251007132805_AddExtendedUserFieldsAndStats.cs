using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameSpace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExtendedUserFieldsAndStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserStatus",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User_Address",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "User_CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "User_birthdate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User_email",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User_phone",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "User_registration_date",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserSignInStat",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SignTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PointsGained = table.Column<int>(type: "int", nullable: false),
                    PointsGainedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpGained = table.Column<int>(type: "int", nullable: false),
                    ExpGainedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CouponGained = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CouponGainedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSignInStat", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_UserSignInStat_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSignInStat_UserId",
                table: "UserSignInStat",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSignInStat");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_birthdate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_email",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_phone",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_registration_date",
                table: "AspNetUsers");
        }
    }
}
