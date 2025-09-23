using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMonitoramento.Migrations
{
    /// <inheritdoc />
    public partial class AddMovimentacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movimentacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MotoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SetorAntigoId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    SetorNovoId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DataHora = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimentacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimentacoes_Motos_MotoId",
                        column: x => x.MotoId,
                        principalTable: "Motos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movimentacoes_Setores_SetorAntigoId",
                        column: x => x.SetorAntigoId,
                        principalTable: "Setores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movimentacoes_Setores_SetorNovoId",
                        column: x => x.SetorNovoId,
                        principalTable: "Setores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_MotoId",
                table: "Movimentacoes",
                column: "MotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_SetorAntigoId",
                table: "Movimentacoes",
                column: "SetorAntigoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_SetorNovoId",
                table: "Movimentacoes",
                column: "SetorNovoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimentacoes");
        }
    }
}
