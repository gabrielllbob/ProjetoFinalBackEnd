using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sprint3.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Scripts", "Dump20260417.sql");
            var sql = File.ReadAllText(path);
            migrationBuilder.Sql(sql, suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP PROCEDURE IF EXISTS spSelTransacoesCliente;
                DROP PROCEDURE IF EXISTS spSelTransacoes;
                DROP PROCEDURE IF EXISTS spSelMensalEmprestimo;
                DROP PROCEDURE IF EXISTS spSelMensalEmprestimos;
                DROP PROCEDURE IF EXISTS spSelEmprestimo;
                DROP PROCEDURE IF EXISTS spSelEmprestimos;
                DROP PROCEDURE IF EXISTS spSelCartao;
                DROP PROCEDURE IF EXISTS spSelCartoes;
                DROP PROCEDURE IF EXISTS spSelConta;
                DROP PROCEDURE IF EXISTS spSelContas;
                DROP PROCEDURE IF EXISTS spSelCliente;
                DROP PROCEDURE IF EXISTS spSelClientes;
                DROP PROCEDURE IF EXISTS spDelMensalEmprestimo;
                DROP PROCEDURE IF EXISTS spDelCartao;
                DROP PROCEDURE IF EXISTS spDelConta;
                DROP PROCEDURE IF EXISTS spDelCliente;
                DROP PROCEDURE IF EXISTS spEdtStatusCartao;
                DROP PROCEDURE IF EXISTS spEdtStatusConta;
                DROP PROCEDURE IF EXISTS spEdtStatusCliente;
                DROP PROCEDURE IF EXISTS spEdtCartao;
                DROP PROCEDURE IF EXISTS spEdtConta;
                DROP PROCEDURE IF EXISTS spEdtCliente;
                DROP PROCEDURE IF EXISTS spInsViaCartao;
                DROP PROCEDURE IF EXISTS spInsTransacao;
                DROP PROCEDURE IF EXISTS spInsMensalEmprestimo;
                DROP PROCEDURE IF EXISTS spInsEmprestimo;
                DROP PROCEDURE IF EXISTS spInsCartao;
                DROP PROCEDURE IF EXISTS spInsConta;
                DROP PROCEDURE IF EXISTS spInsCliente;
                DROP VIEW IF EXISTS vwcartoes;
                DROP TABLE IF EXISTS transacoes;
                DROP TABLE IF EXISTS mensalemprestimos;
                DROP TABLE IF EXISTS emprestimos;
                DROP TABLE IF EXISTS cartoes;
                DROP TABLE IF EXISTS contas;
                DROP TABLE IF EXISTS clientes;
            ", suppressTransaction: true);
        }
    }
}
