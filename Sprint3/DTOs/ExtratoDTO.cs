namespace Sprint3.DTOs;
public class ExtratoDTO {
    public string selTransacao { get; set; } = string.Empty;
    public double nroValor { get; set; }
    public DateTime dtcTransacao { get; set; }
    public string? clienteOrigem { get; set; }
    public string? clienteDestino { get; set; }
}