// <copyright file="20260521222118_InitialSqlServerSchema.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Migrations;

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

/// <inheritdoc />
public partial class InitialSqlServerSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Certificates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CertificateCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                IssuedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Certificates", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                Type = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                RetryCount = table.Column<int>(type: "int", nullable: false),
                Error = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OutboxMessages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SecurityAuditEvents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                EventType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Succeeded = table.Column<bool>(type: "bit", nullable: false),
                Detail = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SecurityAuditEvents", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Role = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                UserType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Expertise = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Courses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Level = table.Column<int>(type: "int", nullable: false),
                DurationHours = table.Column<int>(type: "int", nullable: false),
                DurationMinutes = table.Column<int>(type: "int", nullable: false),
                AverageRatingValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                NumberOfRatings = table.Column<int>(type: "int", nullable: false),
                Category = table.Column<int>(type: "int", nullable: false),
                PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Courses", x => x.Id);
                table.ForeignKey(
                    name: "FK_Courses_Users_InstructorId",
                    column: x => x.InstructorId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "RefreshTokens",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReplacedByTokenHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RevokedReason = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                CreatedByIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_RefreshTokens_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Enrollments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                CompletedDateUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                CourseRatingValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                CourseRatingCount = table.Column<int>(type: "int", nullable: true),
                Review = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ReviewedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Enrollments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Enrollments_Courses_CourseId",
                    column: x => x.CourseId,
                    principalTable: "Courses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Enrollments_Users_StudentId",
                    column: x => x.StudentId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Modules",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Modules", x => x.Id);
                table.ForeignKey(
                    name: "FK_Modules_Courses_CourseId",
                    column: x => x.CourseId,
                    principalTable: "Courses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Progresses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Progresses", x => x.Id);
                table.ForeignKey(
                    name: "FK_Progresses_Enrollments_EnrollmentId",
                    column: x => x.EnrollmentId,
                    principalTable: "Enrollments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Submissions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsGraded = table.Column<bool>(type: "bit", nullable: false),
                Score = table.Column<int>(type: "int", nullable: true),
                Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                GradedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                GradedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Submissions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Submissions_Enrollments_EnrollmentId",
                    column: x => x.EnrollmentId,
                    principalTable: "Enrollments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Assignments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                MaxPoints = table.Column<int>(type: "int", nullable: false),
                DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Assignments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Assignments_Modules_ModuleId",
                    column: x => x.ModuleId,
                    principalTable: "Modules",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Lessons",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                DurationHours = table.Column<int>(type: "int", nullable: false),
                DurationMinutes = table.Column<int>(type: "int", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                createdAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                updatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Lessons", x => x.Id);
                table.ForeignKey(
                    name: "FK_Lessons_Modules_ModuleId",
                    column: x => x.ModuleId,
                    principalTable: "Modules",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Assignments_ModuleId",
            table: "Assignments",
            column: "ModuleId");

        migrationBuilder.CreateIndex(
            name: "IX_Certificates_CertificateCode",
            table: "Certificates",
            column: "CertificateCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Certificates_EnrollmentId",
            table: "Certificates",
            column: "EnrollmentId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Courses_InstructorId",
            table: "Courses",
            column: "InstructorId");

        migrationBuilder.CreateIndex(
            name: "IX_Enrollments_CourseId",
            table: "Enrollments",
            column: "CourseId");

        migrationBuilder.CreateIndex(
            name: "IX_Enrollments_StudentId",
            table: "Enrollments",
            column: "StudentId");

        migrationBuilder.CreateIndex(
            name: "IX_Lessons_ModuleId",
            table: "Lessons",
            column: "ModuleId");

        migrationBuilder.CreateIndex(
            name: "IX_Modules_CourseId",
            table: "Modules",
            column: "CourseId");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessages_ProcessedOnUtc",
            table: "OutboxMessages",
            column: "ProcessedOnUtc");

        migrationBuilder.CreateIndex(
            name: "IX_Progresses_EnrollmentId",
            table: "Progresses",
            column: "EnrollmentId");

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_TokenHash",
            table: "RefreshTokens",
            column: "TokenHash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RefreshTokens_UserId",
            table: "RefreshTokens",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_Submissions_EnrollmentId",
            table: "Submissions",
            column: "EnrollmentId");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Assignments");

        migrationBuilder.DropTable(
            name: "Certificates");

        migrationBuilder.DropTable(
            name: "Lessons");

        migrationBuilder.DropTable(
            name: "OutboxMessages");

        migrationBuilder.DropTable(
            name: "Progresses");

        migrationBuilder.DropTable(
            name: "RefreshTokens");

        migrationBuilder.DropTable(
            name: "SecurityAuditEvents");

        migrationBuilder.DropTable(
            name: "Submissions");

        migrationBuilder.DropTable(
            name: "Modules");

        migrationBuilder.DropTable(
            name: "Enrollments");

        migrationBuilder.DropTable(
            name: "Courses");

        migrationBuilder.DropTable(
            name: "Users");
    }
}
