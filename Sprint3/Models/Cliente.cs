using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("clientes")]
public class Cliente
{
    [Key]
    [Column("ideCliente")]
    public long IdeCliente { get; set; }

    [Column("nomCliente")]
    public string NomCliente { get; set; } = string.Empty;

    [Column("nroCPF")]
    public string NroCPF { get; set; } = string.Empty;

    [Column("nomEndereco")]
    public string NomEndereco { get; set; } = string.Empty;

    [Column("dtcNascimento")]
    public DateTime DtcNascimento { get; set; }

    [Column("stsAtivo")]
    public bool StsAtivo { get; set; }
    
    [Column("senhaHash")]
    public string? SenhaHash { get; set; }

    [Column("role")]
    public string? Role { get; set; }
}