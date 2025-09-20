using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace dotFitness.Modules.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialUsersSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    EventType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EventData = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    TraceId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastError = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMetrics",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Weight = table.Column<double>(type: "numeric(5,2)", nullable: true),
                    Height = table.Column<double>(type: "numeric(5,2)", nullable: true),
                    Bmi = table.Column<double>(type: "numeric(4,2)", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GoogleId = table.Column<string>(type: "varchar(255)", nullable: true),
                    Email = table.Column<string>(type: "varchar(320)", nullable: false),
                    DisplayName = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "varchar(2048)", nullable: true),
                    LoginMethod = table.Column<string>(type: "varchar(50)", nullable: false),
                    Roles = table.Column<string>(type: "jsonb", nullable: false),
                    Gender = table.Column<string>(type: "varchar(50)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    UnitPreference = table.Column<string>(type: "varchar(50)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsOnboarded = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OnboardingCompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AvailableEquipmentIds = table.Column<string>(type: "jsonb", nullable: false),
                    FocusMuscleGroupIds = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedAt",
                schema: "users",
                table: "OutboxMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_EventId",
                schema: "users",
                table: "OutboxMessages",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_EventType",
                schema: "users",
                table: "OutboxMessages",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_IsProcessed",
                schema: "users",
                table: "OutboxMessages",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_IsProcessed_CreatedAt",
                schema: "users",
                table: "OutboxMessages",
                columns: new[] { "IsProcessed", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserMetrics_Bmi",
                schema: "users",
                table: "UserMetrics",
                column: "Bmi",
                filter: "\"Bmi\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserMetrics_CreatedAt",
                schema: "users",
                table: "UserMetrics",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserMetrics_Date",
                schema: "users",
                table: "UserMetrics",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_UserMetrics_UserId",
                schema: "users",
                table: "UserMetrics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMetrics_UserId_Date",
                schema: "users",
                table: "UserMetrics",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                schema: "users",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "users",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleId",
                schema: "users",
                table: "Users",
                column: "GoogleId",
                unique: true,
                filter: "\"GoogleId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsOnboarded",
                schema: "users",
                table: "Users",
                column: "IsOnboarded");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LoginMethod",
                schema: "users",
                table: "Users",
                column: "LoginMethod");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "users");

            migrationBuilder.DropTable(
                name: "UserMetrics",
                schema: "users");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "users");
        }
    }
}
