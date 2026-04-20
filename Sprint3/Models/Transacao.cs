using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("transacoes")]
public class Transacao
{
    [Key]
    [Column("ideTransacao")]
    public long IdeTransacao { get; set; }

    [Column("selTransacao")]
    public string SelTransacao { get; set; } = string.Empty; // 'Depósito','Saque','Transferência'

    [Column("nroValor")]
    public double NroValor { get; set; }

    [Column("dtcTransacao")]
    public DateTime DtcTransacao { get; set; }

    [Column("ideContaOrigem")]
    public long? IdeContaOrigem { get; set; }

    [Column("ideContaFinal")]
    public long? IdeContaFinal { get; set; }
}