using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models
{
    [Table("cartoes")]
    public class Cartao
    {
        [Key]
        [Column("ideCartao")]
        public long Id { get; set; } // BIGINT = long

        [Column("ideConta")]
        public long? ContaId { get; set; } // FK

        [Column("nroCartao")]
        public string NumeroCartao { get; set; } = string.Empty; // CHAR(12)

        [Column("nroVia")]
        public int Via { get; set; } = 1;

        [Column("nroLimite")]
        public int Limite { get; set; } = 0;

        [Column("stsAtivo")]
        public bool Ativo { get; set; } = true;

        [Column("dtcCriacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Column("dtcVencimento")]
        public DateTime DataVencimento { get; set; } = DateTime.Now.AddYears(5);

        [Column("selCartao")]
        public string TipoCartao { get; set; } = string.Empty;
    }
}