// <copyright file="20260522221441_AddProcessedIntegrationMessages.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Migrations;

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

/// <inheritdoc />
public partial class AddProcessedIntegrationMessages : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProcessedIntegrationMessages",
            columns: table => new
            {
                MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Consumer = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Type = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                Error = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProcessedIntegrationMessages", x => new { x.MessageId, x.Consumer });
            });

        migrationBuilder.CreateIndex(
            name: "IX_ProcessedIntegrationMessages_ProcessedOnUtc",
            table: "ProcessedIntegrationMessages",
            column: "ProcessedOnUtc");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProcessedIntegrationMessages");
    }
}
