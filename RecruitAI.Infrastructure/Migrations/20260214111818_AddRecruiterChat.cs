using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruiterChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecruiterChatSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecruiterId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExtractedRequirementJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruiterChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecruiterChatSessions_Recruiters_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Recruiters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecruiterChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsFromRecruiter = table.Column<bool>(type: "boolean", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruiterChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecruiterChatMessages_RecruiterChatSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "RecruiterChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecruiterChatMessages_SessionId",
                table: "RecruiterChatMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruiterChatSessions_RecruiterId",
                table: "RecruiterChatSessions",
                column: "RecruiterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecruiterChatMessages");

            migrationBuilder.DropTable(
                name: "RecruiterChatSessions");
        }
    }
}
