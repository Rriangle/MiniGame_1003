using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameSpace.Areas.MiniGame.Migrations
{
    /// <inheritdoc />
    public partial class AddSignInRuleTableAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 創建 SignInRule 表
            migrationBuilder.CreateTable(
                name: "SignInRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SignInDay = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    HasCoupon = table.Column<bool>(type: "bit", nullable: false),
                    CouponTypeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignInRule", x => x.Id);
                });

            // 創建索引
            migrationBuilder.CreateIndex(
                name: "IX_SignInRule_SignInDay",
                table: "SignInRule",
                column: "SignInDay",
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_SignInRule_IsActive",
                table: "SignInRule",
                column: "IsActive");

            // 插入種子資料
            migrationBuilder.InsertData(
                table: "SignInRule",
                columns: new[] { "SignInDay", "Points", "Experience", "HasCoupon", "CouponTypeCode", "IsActive", "CreatedAt", "Description" },
                values: new object[,]
                {
                    { 1, 10, 5, false, null, true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "第一天簽到獎勵" },
                    { 2, 10, 5, false, null, true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "第二天簽到獎勵" },
                    { 3, 15, 8, false, null, true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "第三天簽到獎勵" },
                    { 4, 15, 8, false, null, true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "第四天簽到獎勵" },
                    { 5, 20, 10, false, null, true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "第五天簽到獎勵" },
                    { 6, 20, 10, false, null, true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "第六天簽到獎勵" },
                    { 7, 30, 15, true, "WEEK_BONUS", true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "第七天簽到獎勵 + 週獎勵優惠券" },
                    { 14, 50, 25, true, "TWO_WEEK_BONUS", true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "連續簽到 14 天獎勵" },
                    { 21, 80, 40, true, "THREE_WEEK_BONUS", true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "連續簽到 21 天獎勵" },
                    { 30, 150, 75, true, "MONTH_BONUS", true, new DateTime(2025, 10, 7, 19, 10, 0, DateTimeKind.Utc), "連續簽到 30 天獎勵（滿月獎勵）" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 刪除表（包含所有資料和索引）
            migrationBuilder.DropTable(name: "SignInRule");
        }
    }
}
