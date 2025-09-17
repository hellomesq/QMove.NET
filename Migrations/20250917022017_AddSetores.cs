using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMonitoramento.Migrations
{
    /// <inheritdoc />
    public partial class AddSetores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Setor",
                table: "Motos");

            migrationBuilder.AddColumn<int>(
                name: "SetorId",
                table: "Motos",
                type: "NUMBER(10)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Setores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Motos_SetorId",
                table: "Motos",
                column: "SetorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Motos_Setores_SetorId",
                table: "Motos",
                column: "SetorId",
                principalTable: "Setores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Motos_Setores_SetorId",
                table: "Motos");

            migrationBuilder.DropTable(
                name: "Setores");

            migrationBuilder.DropIndex(
                name: "IX_Motos_SetorId",
                table: "Motos");

            migrationBuilder.DropColumn(
                name: "SetorId",
                table: "Motos");

            migrationBuilder.AddColumn<string>(
                name: "Setor",
                table: "Motos",
                type: "NVARCHAR2(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
