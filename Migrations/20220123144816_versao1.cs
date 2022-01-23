using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class versao1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdFirstAprovador",
                table: "Aprovacao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdFirstAprovador",
                table: "Aprovacao");
        }
    }
}
