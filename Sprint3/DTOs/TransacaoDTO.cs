namespace Sprint3.DTOs;

public class TransacaoCreateDTO
{
    // Usaremos os mesmos nomes do ENUM no banco: "Depósito", "Saque", "Transferência"
    public string TipoTransacao { get; set; } = string.Empty; 
    public decimal Valor { get; set; }
    public long? ContaOrigemId { get; set; }
    public long? ContaFinalId { get; set; }
}