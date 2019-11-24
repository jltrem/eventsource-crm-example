using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Webapp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregateEvents",
                columns: table => new
                {
                    RootId = table.Column<Guid>(nullable: false),
                    AggregateVersion = table.Column<int>(nullable: false),
                    AggregateName = table.Column<string>(maxLength: 40, nullable: true),
                    EventName = table.Column<string>(maxLength: 40, nullable: true),
                    EventVersion = table.Column<int>(nullable: false),
                    DataType = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Owner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregateEvents", x => new { x.RootId, x.AggregateVersion });
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregateEvents_AggregateName",
                table: "AggregateEvents",
                column: "AggregateName");

            migrationBuilder.CreateIndex(
                name: "IX_AggregateEvents_Timestamp",
                table: "AggregateEvents",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregateEvents");
        }
    }
}
