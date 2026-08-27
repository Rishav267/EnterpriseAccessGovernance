using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseAccessGovernance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificationReviewAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificationReviewAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificationReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActionAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationReviewAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificationReviewAudits_AccessAssignments_AccessAssignmentId",
                        column: x => x.AccessAssignmentId,
                        principalTable: "AccessAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificationReviewAudits_CertificationReviews_CertificationReviewId",
                        column: x => x.CertificationReviewId,
                        principalTable: "CertificationReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificationReviewAudits_Employees_ReviewerEmployeeId",
                        column: x => x.ReviewerEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificationReviewAudits_AccessAssignmentId",
                table: "CertificationReviewAudits",
                column: "AccessAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationReviewAudits_ActionAtUtc",
                table: "CertificationReviewAudits",
                column: "ActionAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationReviewAudits_CertificationReviewId",
                table: "CertificationReviewAudits",
                column: "CertificationReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificationReviewAudits_ReviewerEmployeeId",
                table: "CertificationReviewAudits",
                column: "ReviewerEmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificationReviewAudits");
        }
    }
}
