using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddUserActivityLogRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_PerformedByUserId",
                table: "UserActivityLogs",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_TargetUserId",
                table: "UserActivityLogs",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLogs_Users_PerformedByUserId",
                table: "UserActivityLogs",
                column: "PerformedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLogs_Users_TargetUserId",
                table: "UserActivityLogs",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLogs_Users_PerformedByUserId",
                table: "UserActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLogs_Users_TargetUserId",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserActivityLogs_PerformedByUserId",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserActivityLogs_TargetUserId",
                table: "UserActivityLogs");
        }
    }
}
