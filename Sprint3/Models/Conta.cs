using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models
{
    [Table("contas")] // 👈 Garante que ele procure a tabela com 'c' minúsculo
    public class Conta
    {
        [Key]
        [Column("ideConta")]
        public long Id { get; set; } // BIGINT no MySQL vira long no C#

        [Column("nroSaldo")]
        public double Saldo { get; set; } // DOUBLE no MySQL

        [Column("selConta")]
        public string TipoConta { get; set; } = string.Empty; // 'Empresarial','Poupança','Corrente'

        [Column("ideCliente")]
        public long? ClienteId { get; set; } // BIGINT NULL

        [Column("stsAtivo")]
        public bool Ativo { get; set; } // TINYINT(1) vira bool no C#
    }
}