using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymSystem.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnowSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberRoles",
                columns: table => new
                {
                    MemberRoleID = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MemberRoleName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRoles", x => x.MemberRoleID);
                });

            migrationBuilder.CreateTable(
                name: "PayTypes",
                columns: table => new
                {
                    PayTypeID = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    PayTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayTypes", x => x.PayTypeID);
                });

            migrationBuilder.CreateTable(
                name: "TrainingClasses",
                columns: table => new
                {
                    ClassTypeID = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClassLength = table.Column<int>(type: "int", nullable: false),
                    ClassDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingClasses", x => x.ClassTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MemberPassword = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MemberRole = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    MemberTel = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    LineID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MemberBirthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberGender = table.Column<bool>(type: "bit", nullable: false),
                    MemberAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MemberSource = table.Column<int>(type: "int", nullable: true),
                    MemberRemark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberID);
                    table.ForeignKey(
                        name: "FK_Members_KnowSources_MemberSource",
                        column: x => x.MemberSource,
                        principalTable: "KnowSources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Members_MemberRoles_MemberRole",
                        column: x => x.MemberRole,
                        principalTable: "MemberRoles",
                        principalColumn: "MemberRoleID");
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ContractID = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Signer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MemberID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TrainerID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    HandlerID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    SignDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClassTypeID = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PayTypeID = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ContractID);
                    table.ForeignKey(
                        name: "FK_Contracts_Members_HandlerID",
                        column: x => x.HandlerID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Members_TrainerID",
                        column: x => x.TrainerID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_PayTypes_PayTypeID",
                        column: x => x.PayTypeID,
                        principalTable: "PayTypes",
                        principalColumn: "PayTypeID");
                    table.ForeignKey(
                        name: "FK_Contracts_TrainingClasses_ClassTypeID",
                        column: x => x.ClassTypeID,
                        principalTable: "TrainingClasses",
                        principalColumn: "ClassTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationLogs_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    AttendanceID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ContractID = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    MemberID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TrainerID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.AttendanceID);
                    table.ForeignKey(
                        name: "FK_Attendances_Contracts_ContractID",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ContractID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendances_Members_TrainerID",
                        column: x => x.TrainerID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContractEdits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractID = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    HandlerID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditType = table.Column<int>(type: "int", nullable: false),
                    NewEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddClassCount = table.Column<int>(type: "int", nullable: true),
                    TransferToMemberID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    TransferClassCount = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractEdits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractEdits_Contracts_ContractID",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ContractID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractEdits_Members_HandlerID",
                        column: x => x.HandlerID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingDates",
                columns: table => new
                {
                    TrainingDateID = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ClassDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ContractID = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    TrainerID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingDates", x => x.TrainingDateID);
                    table.ForeignKey(
                        name: "FK_TrainingDates_Contracts_ContractID",
                        column: x => x.ContractID,
                        principalTable: "Contracts",
                        principalColumn: "ContractID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingDates_Members_TrainerID",
                        column: x => x.TrainerID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MemberRoles",
                columns: new[] { "MemberRoleID", "MemberRoleName" },
                values: new object[,]
                {
                    { "A", "會員" },
                    { "B", "教練" },
                    { "C", "後台" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ContractID",
                table: "Attendances",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_MemberID",
                table: "Attendances",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TrainerID",
                table: "Attendances",
                column: "TrainerID");

            migrationBuilder.CreateIndex(
                name: "IX_ContractEdits_ContractID",
                table: "ContractEdits",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_ContractEdits_HandlerID",
                table: "ContractEdits",
                column: "HandlerID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClassTypeID",
                table: "Contracts",
                column: "ClassTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_HandlerID",
                table: "Contracts",
                column: "HandlerID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_MemberID",
                table: "Contracts",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PayTypeID",
                table: "Contracts",
                column: "PayTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_TrainerID",
                table: "Contracts",
                column: "TrainerID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberRole",
                table: "Members",
                column: "MemberRole");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberSource",
                table: "Members",
                column: "MemberSource");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_MemberId",
                table: "OperationLogs",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDates_ContractID",
                table: "TrainingDates",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingDates_TrainerID",
                table: "TrainingDates",
                column: "TrainerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "ContractEdits");

            migrationBuilder.DropTable(
                name: "OperationLogs");

            migrationBuilder.DropTable(
                name: "TrainingDates");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "PayTypes");

            migrationBuilder.DropTable(
                name: "TrainingClasses");

            migrationBuilder.DropTable(
                name: "KnowSources");

            migrationBuilder.DropTable(
                name: "MemberRoles");
        }
    }
}
