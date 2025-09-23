using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMonitoramento.Migrations
{
    /// <inheritdoc />
    public partial class Atualizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_Setores_SetorAntigoId",
                table: "Movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_Setores_SetorNovoId",
                table: "Movimentacoes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Motos");

            migrationBuilder.AlterColumn<int>(
                name: "SetorNovoId",
                table: "Movimentacoes",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SetorAntigoId",
                table: "Movimentacoes",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_Setores_SetorAntigoId",
                table: "Movimentacoes",
                column: "SetorAntigoId",
                principalTable: "Setores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_Setores_SetorNovoId",
                table: "Movimentacoes",
                column: "SetorNovoId",
                principalTable: "Setores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_Setores_SetorAntigoId",
                table: "Movimentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_Setores_SetorNovoId",
                table: "Movimentacoes");

            migrationBuilder.AlterColumn<int>(
                name: "SetorNovoId",
                table: "Movimentacoes",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<int>(
                name: "SetorAntigoId",
                table: "Movimentacoes",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Motos",
                type: "NVARCHAR2(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_Setores_SetorAntigoId",
                table: "Movimentacoes",
                column: "SetorAntigoId",
                principalTable: "Setores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_Setores_SetorNovoId",
                table: "Movimentacoes",
                column: "SetorNovoId",
                principalTable: "Setores",
                principalColumn: "Id");
        }
    }
}
