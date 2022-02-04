using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiTiempo.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoEstacion",
                table: "TiempoItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoEstacion",
                table: "TiempoItems");
        }
    }
}
