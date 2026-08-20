using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaEmissaoNF.Faturamento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "nota_fiscal_numero_seq");

            migrationBuilder.CreateTable(
                name: "nota_fiscal",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero_sequencial = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "char(1)", nullable: false),
                    status_impressao = table.Column<string>(type: "char(1)", nullable: false),
                    pdf_arquivo = table.Column<byte[]>(type: "bytea", nullable: true),
                    pdf_gerado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nota_fiscal", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nota_fiscal_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nota_fiscal_id = table.Column<int>(type: "integer", nullable: false),
                    produto_id = table.Column<int>(type: "integer", nullable: false),
                    produto_codigo = table.Column<long>(type: "bigint", nullable: false),
                    produto_descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nota_fiscal_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_nota_fiscal_item_nota_fiscal_nota_fiscal_id",
                        column: x => x.nota_fiscal_id,
                        principalTable: "nota_fiscal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nota_fiscal_numero_sequencial",
                table: "nota_fiscal",
                column: "numero_sequencial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nota_fiscal_item_nota_fiscal_id_produto_id",
                table: "nota_fiscal_item",
                columns: new[] { "nota_fiscal_id", "produto_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nota_fiscal_item");

            migrationBuilder.DropTable(
                name: "nota_fiscal");

            migrationBuilder.DropSequence(
                name: "nota_fiscal_numero_seq");
        }
    }
}
