using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BidFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddRBACTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    ıd = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    resource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    endpoint_pattern = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ıs_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.ıd);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    ıd = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ıs_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.ıd);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    ıd = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_ıd = table.Column<int>(type: "integer", nullable: false),
                    permission_ıd = table.Column<int>(type: "integer", nullable: false),
                    ıs_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.ıd);
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_permission_ıd",
                        column: x => x.permission_ıd,
                        principalTable: "permissions",
                        principalColumn: "ıd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_role_ıd",
                        column: x => x.role_ıd,
                        principalTable: "roles",
                        principalColumn: "ıd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    ıd = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_ıd = table.Column<int>(type: "integer", nullable: false),
                    role_ıd = table.Column<int>(type: "integer", nullable: false),
                    ıs_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.ıd);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_ıd",
                        column: x => x.role_ıd,
                        principalTable: "roles",
                        principalColumn: "ıd",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_ıd",
                        column: x => x.user_ıd,
                        principalTable: "users",
                        principalColumn: "ıd",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ı_x__permissions__created_at",
                table: "permissions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ı_x__permissions__endpoint_pattern",
                table: "permissions",
                column: "endpoint_pattern");

            migrationBuilder.CreateIndex(
                name: "ı_x__permissions__ıs_active",
                table: "permissions",
                column: "ıs_active");

            migrationBuilder.CreateIndex(
                name: "ı_x__permissions__name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ı_x__permissions__resource__action",
                table: "permissions",
                columns: new[] { "resource", "action" });

            migrationBuilder.CreateIndex(
                name: "ı_x__role_permissions__ıs_active",
                table: "role_permissions",
                column: "ıs_active");

            migrationBuilder.CreateIndex(
                name: "ı_x__role_permissions__permission_ıd",
                table: "role_permissions",
                column: "permission_ıd");

            migrationBuilder.CreateIndex(
                name: "ı_x__role_permissions__role_ıd",
                table: "role_permissions",
                column: "role_ıd");

            migrationBuilder.CreateIndex(
                name: "ı_x__role_permissions__role_ıd__permission_ıd",
                table: "role_permissions",
                columns: new[] { "role_ıd", "permission_ıd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ı_x__roles__created_at",
                table: "roles",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ı_x__roles__ıs_active",
                table: "roles",
                column: "ıs_active");

            migrationBuilder.CreateIndex(
                name: "ı_x__roles__name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ı_x__user_roles__expires_at",
                table: "user_roles",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ı_x__user_roles__ıs_active",
                table: "user_roles",
                column: "ıs_active");

            migrationBuilder.CreateIndex(
                name: "ı_x__user_roles__role_ıd",
                table: "user_roles",
                column: "role_ıd");

            migrationBuilder.CreateIndex(
                name: "ı_x__user_roles__user_ıd",
                table: "user_roles",
                column: "user_ıd");

            migrationBuilder.CreateIndex(
                name: "ı_x__user_roles__user_ıd__role_ıd",
                table: "user_roles",
                columns: new[] { "user_ıd", "role_ıd" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
